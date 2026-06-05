using System.Collections;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using MiraAPI.Events;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Options;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class TOSRoleManagerPatches
{
    private static readonly List<RoleTypes> CrewmateGhostRolePool = [];
    private static readonly List<RoleTypes> ImpostorGhostRolePool = [];
    private static readonly List<RoleTypes> CustomGhostRolePool = [];

    public static bool ReplaceRoleManager;
    private static List<int> LastImps { get; set; } = [];

    private static void GhostRoleSetup()
    {
        // var ghostRoles = RoleManager.Instance.AllRoles.Where(x => x.IsDead);
        var ghostRoles = Utils.GetRegisteredGhostRoles();

        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Warning($"GhostRoleSetup - ghostRoles Count: {ghostRoles.Count()}");
        CrewmateGhostRolePool.Clear();
        ImpostorGhostRolePool.Clear();
        CustomGhostRolePool.Clear();

        foreach (var role in ghostRoles)
        {
            if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Warning($"GhostRoleSetup - ghostRoles role NiceName: {role.NiceName}");
            var data = Utils.GetAssignData(role.Role);

            switch (data.Chance)
            {
                case 100:
                {
                    if (data.Count > 0)
                    {
                        if (role is ICustomRole { Team: ModdedRoleTeams.Custom })
                        {
                            CustomGhostRolePool.Add(role.Role);
                        }
                        else
                        {
                            switch (role.TeamType)
                            {
                                case RoleTeamTypes.Crewmate:
                                    CrewmateGhostRolePool.Add(role.Role);
                                    break;
                                case RoleTeamTypes.Impostor:
                                    ImpostorGhostRolePool.Add(role.Role);
                                    break;
                            }
                        }
                    }

                    break;
                }
                case > 0:
                {
                    if (data.Count > 0 && HashRandom.Next(101) < data.Chance)
                    {
                        if (role is ICustomRole { Team: ModdedRoleTeams.Custom })
                        {
                            CustomGhostRolePool.Add(role.Role);
                        }
                        else
                        {
                            switch (role.TeamType)
                            {
                                case RoleTeamTypes.Crewmate:
                                    CrewmateGhostRolePool.Add(role.Role);
                                    break;
                                case RoleTeamTypes.Impostor:
                                    ImpostorGhostRolePool.Add(role.Role);
                                    break;
                            }
                        }
                    }

                    break;
                }
            }
        }
    }

    private static void AssignRoles(List<NetworkedPlayerInfo> infected)
    {
        var impCount = infected.Count;
        var impostors = Utils.GetImpostors(infected);
        var crewmates = Utils.GetCrewmates(impostors);

        var nbCount = Random.RandomRange((int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralBenign,
            (int)OptionGroupSingleton<RoleOptions>.Instance.MaxNeutralBenign + 1);
        var neCount = Random.RandomRange((int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralEvil,
            (int)OptionGroupSingleton<RoleOptions>.Instance.MaxNeutralEvil + 1);
        var nkCount = Random.RandomRange((int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralKiller,
            (int)OptionGroupSingleton<RoleOptions>.Instance.MaxNeutralKiller + 1);

        var factions = new List<string> { "Benign", "Evil", "Killing" };

        // Crew must always start out outnumbering neutrals, so subtract roles until that can be guaranteed.
        while (Math.Ceiling((double)crewmates.Count / 2) <= nbCount + neCount + nkCount)
        {
            var canSubtractBenign = CanSubtract(nbCount,
                (int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralBenign);
            var canSubtractEvil =
                CanSubtract(neCount, (int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralEvil);
            var canSubtractKilling = CanSubtract(nkCount,
                (int)OptionGroupSingleton<RoleOptions>.Instance.MinNeutralKiller);
            var canSubtractNone = !canSubtractBenign && !canSubtractEvil && !canSubtractKilling;

            factions.Shuffle();
            switch (factions[0])
            {
                case "Benign":
                    if (nbCount > 0 && (canSubtractBenign || canSubtractNone))
                    {
                        nbCount -= 1;
                        break;
                    }

                    goto case "Evil";
                case "Evil":
                    if (neCount > 0 && (canSubtractEvil || canSubtractNone))
                    {
                        neCount -= 1;
                        break;
                    }

                    goto case "Killing";
                case "Killing":
                    if (nkCount > 0 && (canSubtractKilling || canSubtractNone))
                    {
                        nkCount -= 1;
                        break;
                    }

                    goto default;
                default:
                    if (nbCount > 0)
                    {
                        nbCount -= 1;
                    }
                    else if (neCount > 0)
                    {
                        neCount -= 1;
                    }
                    else if (nkCount > 0)
                    {
                        nkCount -= 1;
                    }

                    break;
            }

            if (nbCount + neCount + nkCount == 0)
            {
                break;
            }
        }
        Func<RoleBehaviour, bool>? impFilter = null;
        if ((MapNames)GameOptionsManager.Instance.GameHostOptions.MapId == MapNames.Fungle)
        {
            impFilter = x => x.Role != (RoleTypes)RoleId.Get<VenererRole>();
        }

        var impRoles =
            Utils.GetMaxRolesToAssign(ModdedRoleTeams.Impostor, impCount, impFilter);

        var nbRoles = Utils.GetMaxRolesToAssign(RoleAlignment.NeutralBenign, nbCount);
        var neRoles = Utils.GetMaxRolesToAssign(RoleAlignment.NeutralEvil, neCount);
        var nkRoles = Utils.GetMaxRolesToAssign(RoleAlignment.NeutralKilling, nkCount);

        var crewCount = crewmates.Count - nbRoles.Count - neRoles.Count - nkRoles.Count;

        Func<RoleBehaviour, bool>? crewFilter = null;
        if (impostors.Count <= 1)
        {
            crewFilter = x => x.Role != (RoleTypes)RoleId.Get<SpyRole>();
        }

        var crewRoles = Utils.GetMaxRolesToAssign(ModdedRoleTeams.Crewmate, crewCount, crewFilter);

        var crewAndNeutRoles = new List<ushort>();
        crewAndNeutRoles.AddRange(nbRoles);
        crewAndNeutRoles.AddRange(neRoles);
        crewAndNeutRoles.AddRange(nkRoles);
        crewAndNeutRoles.AddRange(crewRoles);
        crewAndNeutRoles.Shuffle();

        foreach (var role in crewAndNeutRoles)
        {
            var num = HashRandom.FastNext(crewmates.Count);
            var player = crewmates[num];

            player.RpcSetRole((RoleTypes)role);

            crewmates.RemoveAt(num);

            if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Warning($"SelectRoles - player: '{player.Data.PlayerName}', role: '{RoleManager.Instance.GetRole((RoleTypes)role).NiceName}'");
        }

        foreach (var role in impRoles)
        {
            var num = HashRandom.FastNext(impostors.Count);
            var player = impostors[num];

            player.RpcSetRole((RoleTypes)role);

            impostors.RemoveAt(num);

            if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Warning($"SelectRoles - player: '{player.Data.PlayerName}', role: '{RoleManager.Instance.GetRole((RoleTypes)role).NiceName}'");
        }

        foreach (var player in crewmates)
        {
            player.RpcSetRole(RoleTypes.Crewmate);
        }

        foreach (var player in impostors)
        {
            player.RpcSetRole(RoleTypes.Impostor);
        }

        static bool CanSubtract(int faction, int minFaction)
        {
            return faction > minFaction;
        }
    }

    public static void AssignTargets()
    {
        // This is a coroutine because otherwise, the game just assigns targets real badly like exe being lovers with their targets, that sort of thing - Atony
        Coroutines.Start(CoAssignTargets());
    }

    public static IEnumerator CoAssignTargets()
    {
        foreach (var role in Utils.AllRoles.Where(x => x is IAssignableTargets)
                     .OrderBy(x => (x as IAssignableTargets)!.Priority))
        {
            if (role is IAssignableTargets assignRole)
            {
                assignRole.AssignTargets();
                yield return new WaitForSeconds(0.01f);
            }
        }

        foreach (var modifier in Utils.AllModifiers.Where(x => x is IAssignableTargets)
                     .OrderBy(x => (x as IAssignableTargets)!.Priority))
        {
            if (modifier is IAssignableTargets assignMod)
            {
                assignMod.AssignTargets();
                yield return new WaitForSeconds(0.01f);
            }
        }

        GhostRoleSetup();
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool SelectRolesPatch(RoleManager __instance)
    {
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Error($"RoleManager.SelectRoles - ReplaceRoleManager: {ReplaceRoleManager}");

        if (TutorialManager.InstanceExists || ReplaceRoleManager)
        {
            return true;
        }

        var players = GameData.Instance.AllPlayers.ToArray().ToList();
        players.Shuffle();

        var impCount = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(players.Count);
        List<NetworkedPlayerInfo> infected = [];

        infected.AddRange(players.Take(impCount));

        LastImps = [.. infected.Select(x => x.ClientId)];
       
        AssignRoles(infected);
        AssignTargets();

        return false;
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetRole))]
    [HarmonyPrefix]
    public static bool RpcSetRolePatch(PlayerControl __instance, [HarmonyArgument(0)] RoleTypes roleType,
        [HarmonyArgument(1)] bool canOverrideRole = false)
    {
        if (AmongUsClient.Instance.AmClient)
        {
            __instance.StartCoroutine(__instance.CoSetRole(roleType, canOverrideRole));
        }

        var messageWriter =
            AmongUsClient.Instance.StartRpcImmediately(__instance.NetId, (byte)RpcCalls.SetRole, SendOption.Reliable);
        messageWriter.Write((ushort)roleType);
        messageWriter.Write(canOverrideRole);
        AmongUsClient.Instance.FinishRpcImmediately(messageWriter);

        var changeRoleEvent = new ChangeRoleEvent(__instance, null, RoleManager.Instance.GetRole(roleType));
        MiraEventManager.InvokeEvent(changeRoleEvent);

        return false;
    }

    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.TryAssignSpecialGhostRoles))]
    [HarmonyPrefix]
    public static bool TryAssignSpecialGhostRolesPatch(RoleManager __instance, PlayerControl player)
    {
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Warning($"TryAssignSpecialGhostRolesPatch - Player: '{player.Data.PlayerName}'");
        var ghostRole = RoleTypes.CrewmateGhost;

        if (player.Is(Factions.Crewmate) && CrewmateGhostRolePool.Count > 0)
        {
            ghostRole = CrewmateGhostRolePool.TakeFirst();
        }
        else if (player.IsImpostor() && ImpostorGhostRolePool.Count > 0)
        {
            ghostRole = ImpostorGhostRolePool.TakeFirst();
        }
        else if (player.Is(Factions.Neutral) && CustomGhostRolePool.Count > 0)
        {
            ghostRole = CustomGhostRolePool.TakeFirst();
        }

        if (ghostRole != RoleTypes.CrewmateGhost && ghostRole != RoleTypes.ImpostorGhost &&
            ghostRole != (RoleTypes)RoleId.Get<NeutralGhostRole>())
            // var newRole = RoleManager.Instance.GetRole(ghostRole);
            // Logger<TownOfSushiPlugin>.Message($"TryAssignSpecialGhostRolesPatch - ghostRoles role: {newRole.NiceName}");
        {
            player.RpcChangeRole((ushort)ghostRole);
        }

        return false;
    }
}