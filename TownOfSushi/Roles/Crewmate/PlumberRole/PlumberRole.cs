﻿using System.Collections;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using UnityEngine;
using System.Globalization;

namespace TownOfSushi.Roles.Crewmate;

public sealed class PlumberRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    [HideFromIl2Cpp] public List<int> FutureBlocks { get; set; } = [];
    // Blocked vent, remaining rounds
    [HideFromIl2Cpp] public static List<KeyValuePair<int, int>> VentsBlocked { get; set; } = [];

    // Barricade object, remaining rounds
    [HideFromIl2Cpp] public static List<KeyValuePair<GameObject, int>> Barricades { get; set; } = [];

    public string RoleName => "Plumber";
    public string RoleDescription => "Get The Rats Out Of The Sewers";

    public string RoleLongDescription =>
        "Flush the vent system to kick venters out, and\nbarricade vents to block them the next round";

    public Color RoleColor => TownOfSushiColors.Plumber;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Engineer),
        Icon = TOSRoleIcons.Plumber
    };

    public void LobbyStart()
    {
        Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        var duration = (int)OptionGroupSingleton<PlumberOptions>.Instance.BarricadeRoundDuration;
        var text = duration == 0 ? "Barricades Stay Forever." : $"Barricades Stay For {duration} Round(s)";
        stringB.Append(CultureInfo.InvariantCulture,
            $"\n<b><size=60%>Note: {text}</size></b>");
        if (VentsBlocked.Count > 0 || FutureBlocks.Count > 0)
        {
            stringB.Append(CultureInfo.InvariantCulture,
                $"\n<b>Vents List:</b>");

            if (VentsBlocked.Count > 0)
            {
                foreach (var ventPair in VentsBlocked)
                {
                    var vent = Helpers.GetVentById(ventPair.Key);
                    if (vent == null) continue;
                    var text2 = duration == 0 ? string.Empty : $": {ventPair.Value} Round(s) Remaining";
                    stringB.Append(CultureInfo.InvariantCulture,
                        $"\n{MiscUtils.GetRoomName(vent.transform.position)} Vent{text2}");
                }
            }
            if (FutureBlocks.Count > 0)
            {
                foreach (var ventId in FutureBlocks)
                {
                    var vent = Helpers.GetVentById(ventId);
                    if (vent == null) continue;
                        
                    stringB.Append(CultureInfo.InvariantCulture,
                        $"\n<color=#BFBFBF>{MiscUtils.GetRoomName(vent.transform.position)} Vent: Preparing...</color>");
                }
            }
        }

        return stringB;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (TutorialManager.InstanceExists) SubClear();
    }

    public string GetAdvancedDescription()
    {
        return
            "The Plumber is a Crewmate Support role that can place Barricades on vents and Flush anyone out of vents."
            + MiscUtils.AppendOptionsText(GetType());
    }
    public void SubClear()
    {
        FutureBlocks.Clear();
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Flush",
            "Flushing the vents makes every vent open and close, kicking out anyone who is actively in a vent. The Plumber also gets an arrow pointing to every flushed player for one second.",
            TOSCrewAssets.FlushSprite),
        new("Barricade",
            "Barricading a vent places a barricade on the vent selected for the next round, preventing players from using it.",
            TOSCrewAssets.BarricadeSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        foreach (var barricade in Barricades.Select(x => x.Key))
        {
            Destroy(barricade);
        }

        FutureBlocks.Clear();
        VentsBlocked.Clear();
        Barricades.Clear();
    }

    public void SetupBarricades()
    {
        foreach (var ventId in FutureBlocks)
        {
            VentsBlocked.Add(new(ventId, (int)OptionGroupSingleton<PlumberOptions>.Instance.BarricadeRoundDuration));

            GameObject barricade = new("Barricade");

            var trueVent = Helpers.GetVentById(ventId);

            if (trueVent == null)
            {
                continue;
            }

            barricade.transform.SetParent(trueVent.transform);
            barricade.gameObject.layer = trueVent.gameObject.layer;

            var render = barricade.AddComponent<SpriteRenderer>();

            switch (ShipStatus.Instance.Type)
            {
                case ShipStatus.MapType.Fungle:
                    render.sprite = TOSAssets.BarricadeFungleSprite.LoadAsset();
                    barricade.transform.localPosition = new Vector3(0.03f, -0.107f, -0.001f);
                    break;
                case ShipStatus.MapType.Pb:
                    render.sprite = TOSAssets.BarricadeVentSprite.LoadAsset();
                    barricade.transform.localPosition = new Vector3(0, 0.05f, -0.001f);
                    barricade.transform.localScale = new Vector3(0.8f, 0.7f, 1f);
                    break;
                default:
                    render.sprite = TOSAssets.BarricadeVentSprite.LoadAsset();
                    barricade.transform.localPosition = new Vector3(0, 0, -0.001f);
                    break;
            }

            if (trueVent.gameObject.name == "LowerCentralVent" && ModCompatibility.IsSubmerged())
            {
                barricade.transform.localPosition = new Vector3(0, 0.7f, -0.001f);
                barricade.transform.localScale = new Vector3(1.05f, 1.15f, 1.0625f);
            }

            if (ModCompatibility.IsLevelImpostor())
            {
                switch (ModCompatibility.GetLIVentType(trueVent))
                {
                    case "util-vent3":
                        render.sprite = TOSAssets.BarricadeFungleSprite.LoadAsset();
                        barricade.transform.localPosition = new Vector3(0.03f, -0.107f, -0.001f);
                        break;
                    case "util-vent2":
                        render.sprite = TOSAssets.BarricadeVentSprite.LoadAsset();
                        barricade.transform.localPosition = new Vector3(0, 0.05f, -0.001f);
                        barricade.transform.localScale = new Vector3(0.8f, 0.7f, 1f);
                        break;
                    default:
                        render.sprite = TOSAssets.BarricadeVentSprite.LoadAsset();
                        barricade.transform.localPosition = new Vector3(0, 0, -0.001f);
                        break;
                }
            }

            Barricades.Add(new (barricade, (int)OptionGroupSingleton<PlumberOptions>.Instance.BarricadeRoundDuration));
        }

        FutureBlocks.Clear();
    }

    public static IEnumerator SeeVenter(PlayerControl plumber)
    {
        var playersInVent = PlayerControl.AllPlayerControls.ToArray().Where(x => x.inVent);

        foreach (var player in playersInVent)
        {
            player.AddModifier<PlumberVenterModifier>(plumber, Color.white);
        }

        yield return new WaitForSeconds(1f);

        foreach (var player in ModifierUtils.GetPlayersWithModifier<PlumberVenterModifier>(x => x.Owner == plumber))
        {
            player.RemoveModifier<PlumberVenterModifier>();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.PlumberFlush, SendImmediately = true)]
    public static void RpcPlumberFlush(PlayerControl player)
    {
        if (player.Data.Role is not PlumberRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlumberFlush - Invalid Plumber");
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PlumberFlush, player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (PlayerControl.LocalPlayer.inVent)
        {
            PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
            PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();

            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Plumber));
        }

        if (!player.AmOwner)
        {
            return;
        }

        var someoneInVent = PlayerControl.AllPlayerControls.ToArray().Any(x => x.inVent);
        if (!someoneInVent)
        {
            return;
        }

        Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Plumber));
        Coroutines.Start(SeeVenter(player));
    }

    [MethodRpc((uint)TownOfSushiRpc.PlumberBlockVent, SendImmediately = true)]
    public static void RpcPlumberBlockVent(PlayerControl player, int ventId)
    {
        if (player.Data.Role is not PlumberRole plumber)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlumberBlockVent - Invalid Plumber");
            return;
        }

        if (!plumber.FutureBlocks.Contains(ventId))
        {
            plumber.FutureBlocks.Add(ventId);
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PlumberBlock, player, Helpers.GetVentById(ventId));
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}