using System.Collections;
using System.Text;
using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Game;
using UnityEngine;
using Random = System.Random;
using MiraAPI.Modifiers.Types;

namespace TownOfSushi.Roles.Neutral;

public sealed class GuardianAngelTOSRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue,
    IAssignableTargets, ICrewVariant
{
    public PlayerControl? Target { get; set; }
    public int Priority { get; set; } = 1;
    public MysticClueType MysticHintType => MysticClueType.Protective;

    public void AssignTargets()
    {
        // Logger<TownOfSushiPlugin>.Error($"SelectGATargets");
        var evilTargetPercent = (int)OptionGroupSingleton<GuardianAngelOptions>.Instance.EvilTargetPercent;

        var gas = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => x.IsRole<GuardianAngelTOSRole>() && !x.HasDied());

        foreach (var ga in gas)
        {
            var filtered = PlayerControl.AllPlayerControls.ToArray()
                .Where(x => !x.IsRole<GuardianAngelTOSRole>() && !x.HasDied() &&
                            !x.HasModifier<ExecutionerTargetModifier>() && !x.HasModifier<AllianceGameModifier>())
                .ToList();

            if (evilTargetPercent > 0f)
            {
                Random rnd = new();
                var chance = rnd.Next(1, 101);

                if (chance <= evilTargetPercent)
                {
                    filtered = [.. filtered.Where(x => x.IsImpostor() || x.Is(RoleAlignment.NeutralKilling))];
                }
            }
            else
            {
                filtered = [.. filtered.Where(x => x.Is(ModdedRoleTeams.Crewmate))];
            }

            filtered = [.. filtered.Where(x => !x.Is(RoleAlignment.NeutralEvil))];

            Random rndIndex = new();
            var randomTarget = filtered[rndIndex.Next(0, filtered.Count)];

            // Logger<TownOfSushiPlugin>.Info($"Setting GA Target: {randomTarget.Data.PlayerName}");
            RpcSetGATarget(ga, randomTarget);
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ClericRole>());
    public string RoleName => "Guardian Angel";
    public string RoleDescription => TargetString();
    public string RoleLongDescription => TargetString();
    public Color RoleColor => TownOfSushiColors.GuardianAngel;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.GuardianAngel,
        IntroSound = TOSAudio.GuardianAngelSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public bool SetupIntroTeam(IntroCutscene instance,
        ref Il2CppSystem.Collections.Generic.List<PlayerControl> yourTeam)
    {
        if (Player != PlayerControl.LocalPlayer)
        {
            return true;
        }

        var gaTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

        gaTeam.Add(PlayerControl.LocalPlayer);
        gaTeam.Add(Target!);

        yourTeam = gaTeam;

        return true;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Protect",
            "Protect your target from getting killed.",
            TOSNeutAssets.ProtectSprite)
    ];

    public string GetAdvancedDescription()
    {
        return
            "The Guardian Angel is a Neutral Benign that needs to protect their target (signified by <color=#B3FFFFFF>★</color>) from getting killed/ejected." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (TutorialManager.InstanceExists && Target == null && Player.AmOwner && Player.IsHost() &&
            AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started)
        {
            Coroutines.Start(SetTutorialTargets(this));
        }
    }

    private static IEnumerator SetTutorialTargets(GuardianAngelTOSRole ga)
    {
        yield return new WaitForSeconds(0.01f);
        ga.AssignTargets();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (TutorialManager.InstanceExists && Player.AmOwner)
        {
            var players = ModifierUtils
                .GetPlayersWithModifier<GuardianAngelTargetModifier>([HideFromIl2Cpp] (x) => x.OwnerId == Player.PlayerId)
                .ToList();
            players.Do(x => x.RpcRemoveModifier<GuardianAngelTargetModifier>());
        }
        if (!Player.HasModifier<BasicGhostModifier>() && Player.HasDied())
        {
            Player.AddModifier<BasicGhostModifier>();
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        var gaMod = ModifierUtils.GetActiveModifiers<GuardianAngelTargetModifier>().FirstOrDefault(x => x.OwnerId == Player.PlayerId);
        if (gaMod == null)
        {
            return false;
        }
        return gaMod.Player.Data.Role.DidWin(gameOverReason) || gaMod.Player.GetModifiers<GameModifier>().Any(x => x.DidWin(gameOverReason) == true);
    }

    public static bool GASeesRoleVisibilityFlag(PlayerControl player)
    {
        var gaKnowsTargetRole = OptionGroupSingleton<GuardianAngelOptions>.Instance.GAKnowsTargetRole &&
        PlayerControl.LocalPlayer.IsRole<GuardianAngelTOSRole>() &&
        PlayerControl.LocalPlayer.GetRole<GuardianAngelTOSRole>()!.Target == player;                    

        var targetKnowsGA = OptionGroupSingleton<GuardianAngelOptions>.Instance.TargetKnowsGA &&
        player.IsRole<GuardianAngelTOSRole>() &&
        player.GetRole<GuardianAngelTOSRole>()!.Target == PlayerControl.LocalPlayer;

        return gaKnowsTargetRole || targetKnowsGA;
    }

    public static bool GATargetSeesVisibilityFlag(PlayerControl player)
    {
        var gaTargetKnows =
            OptionGroupSingleton<GuardianAngelOptions>.Instance.ShowProtect is ProtectOptions.SelfAndGA &&
            player.HasModifier<GuardianAngelTargetModifier>();

        var gaKnowsTargetRole = PlayerControl.LocalPlayer.IsRole<GuardianAngelTOSRole>() &&
                                PlayerControl.LocalPlayer.GetRole<GuardianAngelTOSRole>()!.Target == player;

        return gaTargetKnows || gaKnowsTargetRole;
    }

    public void CheckTargetDeath(PlayerControl victim)
    {
        if (Player.HasDied())
        {
            return;
        }

        // Logger<TownOfSushiPlugin>.Error($"OnPlayerDeath '{victim.Data.PlayerName}'");
        if (Target == null || victim == Target)
        {
            var roleType = OptionGroupSingleton<GuardianAngelOptions>.Instance.OnTargetDeath switch
            {
                BecomeOptions.Crew => (ushort)RoleTypes.Crewmate,
                BecomeOptions.Jester => RoleId.Get<JesterRole>(),
                BecomeOptions.Amnesiac => RoleId.Get<AmnesiacRole>(),
                BecomeOptions.Romantic => RoleId.Get<RomanticRole>(),
                BecomeOptions.Thief => RoleId.Get<ThiefRole>(),
                _ => (ushort)RoleTypes.Crewmate
            };

            // Logger<TownOfSushiPlugin>.Error($"OnPlayerDeath - ChangeRole: '{roleType}'");
            Player.ChangeRole(roleType);

            if ((roleType == RoleId.Get<JesterRole>() && OptionGroupSingleton<JesterOptions>.Instance.ScatterOn) ||
                (roleType == RoleId.Get<AmnesiacRole>() && OptionGroupSingleton<AmnesiacOptions>.Instance.ScatterOn))
            {
                StartCoroutine(Effects.Lerp(0.2f,
                    new Action<float>(p => { Player.GetModifier<ScatterModifier>()?.OnRoundStart(); })));
            }
        }
    }

    private string TargetString()
    {
        if (!Target)
        {
            return "Protect Your Target With Your Life!";
        }

        return $"Protect {Target?.Data.PlayerName} With Your Life!";
    }

    [MethodRpc((uint)TownOfSushiRpc.SetGATarget, SendImmediately = true)]
    public static void RpcSetGATarget(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not GuardianAngelTOSRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcSetGATarget - Invalid guardian angel");
            return;
        }

        if (target == null)
        {
            return;
        }

        var role = player.GetRole<GuardianAngelTOSRole>();

        if (role == null)
        {
            return;
        }

        // Logger<TownOfSushiPlugin>.Message($"RpcSetGATarget - Target: '{target.Data.PlayerName}'");
        role.Target = target;

        target.AddModifier<GuardianAngelTargetModifier>(player.PlayerId);
    }
}