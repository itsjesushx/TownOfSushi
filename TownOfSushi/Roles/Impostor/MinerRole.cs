using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class MinerRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Miner";
    public string RoleDescription => "From The Top, Make It Drop, That's A Vent";
    public string RoleLongDescription => "Place interconnected vents around the map";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<EngineerTouRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TosRoleIcons.Miner,
        OptionsScreenshot = TosImpAssets.MinerRoleBanner,
        IntroSound = TosAudio.MineSound,
    };
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not JanitorRole || Player.HasDied() || !Player.AmOwner || MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled && !HudManager.Instance.PetButton.isActiveAndEnabled)) return;
        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<MinerOptions>.Instance.MinerKill || (Player != null && Player.GetModifiers<BaseModifier>().Any(x => x is ICachedRole)) || (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Mine",
            "Place a vent where you are standing. These vents won't connect to already existing vents on the map but with each other.",
            TosImpAssets.MineSprite)
    ];

    [HideFromIl2Cpp]
    public List<Vent> Vents { get; set; } = [];

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        if (OptionGroupSingleton<MinerOptions>.Instance.MineVisibility is MineVisiblityOptions.AfterUse) stringB.Append(CultureInfo.InvariantCulture, $"Vents will only be visible once used");
        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return "The Miner is an Impostor Support role that can create vents." + MiscUtils.AppendOptionsText(GetType());
    }

    [MethodRpc((uint)TownOfSushiRpc.PlaceVent, SendImmediately = true)]
    public static void RpcPlaceVent(PlayerControl player, int ventId, Vector2 position, float zAxis, bool immediate)
    {
        if (player.Data.Role is not MinerRole miner)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlaceVent - Invalid miner");
            return;
        }

        //Logger<TownOfSushiPlugin>.Error("RpcPlaceVent");

        var ventPrefab = ShipStatus.Instance.AllVents[0];
        var vent = Instantiate(ventPrefab, ventPrefab.transform.parent);
        vent.name = $"MinerVent-{player.PlayerId}-{ventId}";

        Logger<TownOfSushiPlugin>.Error($"RpcPlaceVent - vent: {vent.name} - {immediate}");

        if (!player.AmOwner && !immediate)
        {
            Logger<TownOfSushiPlugin>.Error("RpcPlaceVent - Hide Vent");
            vent.myRend.enabled = false;
        }

        vent.Id = ventId;
        vent.transform.position = new Vector3(position.x, position.y, zAxis);

        if (miner == null) return;

        if (miner.Vents.Count > 0)
        {
            var leftVent = miner.Vents[^1];
            vent.Left = leftVent;
            leftVent.Right = vent;
        }
        else
        {
            vent.Left = null;
        }

        vent.Right = null;
        vent.Center = null;

        var allVents = ShipStatus.Instance.AllVents.ToList();
        allVents.Add(vent);
        ShipStatus.Instance.AllVents = allVents.ToArray();

        miner.Vents.Add(vent);

        if (ModCompatibility.SubLoaded)
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); // just in case elevator vent is not blocked
            if (vent.gameObject.transform.position.y > -7)
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
            }
            else
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
            }
        }
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MinerPlaceVent, player, vent);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
        if (immediate)
        {
            var TosAbilityEvent2 = new TosAbilityEvent(AbilityType.MinerRevealVent, player, vent);
            MiraEventManager.InvokeEvent(TosAbilityEvent2);
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.ShowVent, SendImmediately = true)]
    public static void RpcShowVent(PlayerControl player, int ventId)
    {
        if (player.Data.Role is not MinerRole miner)
        {
            Logger<TownOfSushiPlugin>.Error("RpcShowVent - Invalid miner");
            return;
        }

        var vent = miner.Vents.FirstOrDefault(x => x.Id == ventId);

        if (vent != null)
        {
            vent.myRend.enabled = true;

            var TosAbilityEvent = new TosAbilityEvent(AbilityType.MinerRevealVent, player, vent);
            MiraEventManager.InvokeEvent(TosAbilityEvent);
        }
    }
}
