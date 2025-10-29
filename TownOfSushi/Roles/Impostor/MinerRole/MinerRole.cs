
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class MinerRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    [HideFromIl2Cpp] public List<Vent> Vents { get; set; } = [];

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not MinerRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<MinerOptions>.Instance.MinerKill ||
                                                     (Player != null && Player.GetModifiers<BaseModifier>()
                                                         .Any(x => x is ICachedRole)) ||
                                                     (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<EngineerTOSRole>());
    public string RoleName => "Miner";
    public string RoleDescription => "From The Top, Make It Drop, That's A Vent.";
    public string RoleLongDescription => "Place interconnected vents around the map";
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TOSRoleIcons.Miner,
        OptionsScreenshot = TOSImpAssets.MinerRoleBanner,
        IntroSound = TOSAudio.MineSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        if (OptionGroupSingleton<MinerOptions>.Instance.MineVisibility is MineVisiblityOptions.AfterUse)
        {
            stringB.Append(TownOfSushiPlugin.Culture, $"Vents will only be visible once used");
        }

        return stringB;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Mine",
            "Place a vent where you are standing. These vents won't connect to already existing vents on the map but with each other.",
            TOSImpAssets.MineSprite)
    ];

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

        if (miner == null)
        {
            return;
        }

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
        
        PlainShipRoom? plainShipRoom = null;

        var allRooms2 = ShipStatus.Instance.FastRooms;
        foreach (var plainShipRoom2 in allRooms2.Values)
        {
            if (plainShipRoom2.roomArea && plainShipRoom2.roomArea.OverlapPoint(vent.transform.position))
            {
                plainShipRoom = plainShipRoom2;
            }
        }
        
        var mapId = (MapNames)GameOptionsManager.Instance.currentNormalGameOptions.MapId;
        if (TutorialManager.InstanceExists)
        {
            mapId = (MapNames)AmongUsClient.Instance.TutorialMapId;
        }

        if (mapId is MapNames.Polus && plainShipRoom?.RoomId is SystemTypes.Weapons)
        {
            vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                vent.gameObject.transform.position.y, -0.0209f);
        }

        if (ModCompatibility.SubLoaded)
        {
            vent.gameObject.layer = 12;
            vent.gameObject.AddSubmergedComponent("ElevatorMover"); // just in case elevator vent is not blocked
            if (vent.gameObject.transform.position.y > -7)
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.03f);
            }
            else
            {
                vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x,
                    vent.gameObject.transform.position.y, 0.0009f);
                vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x,
                    vent.gameObject.transform.localPosition.y, -0.003f);
            }
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.MinerPlaceVent, player, vent);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        if (immediate)
        {
            var TOSAbilityEvent2 = new TOSAbilityEvent(AbilityType.MinerRevealVent, player, vent);
            MiraEventManager.InvokeEvent(TOSAbilityEvent2);
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

            var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.MinerRevealVent, player, vent);
            MiraEventManager.InvokeEvent(TOSAbilityEvent);
        }
    }
}