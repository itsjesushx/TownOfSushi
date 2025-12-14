using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Buttons;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class EngineerTOSRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Engineer";
    public string RoleDescription => "Vent around and fix sabotages";
    public string RoleLongDescription => "Vent around and fix sabotages remotely";
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public Color RoleColor => TownOfSushiColors.Engineer;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        // CanUseVent = true,
        Icon = TOSRoleIcons.Engineer,
        OptionsScreenshot = TOSCrewAssets.EngineerRoleBanner,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Engineer)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Support role that can vent and fix sabotages remotely."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Fix",
            $"It doesn't matter where you are on the map, you can use your fix button to instantly fix the active sabotage. You can do this {OptionGroupSingleton<EngineerOptions>.Instance.MaxFixes} times per game.",
            TOSCrewAssets.FixButtonSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<FakeVentButton>.Instance.Show = false;
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<FakeVentButton>.Instance.Show = true;
        }
    }

    public static void EngineerFix(PlayerControl engineer)
    {
        switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
        {
            case 0:
            case 3:
                var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (comms1.IsActive)
                {
                    FixComms();
                }

                var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactor1.IsActive)
                {
                    FixReactor(SystemTypes.Reactor);
                }

                var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (oxygen1.IsActive)
                {
                    FixOxygen();
                }

                var lights1 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights1.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                break;
            case 1:
                var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                if (comms2.IsActive)
                {
                    FixMiraComms();
                }

                var reactor2 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactor2.IsActive)
                {
                    FixReactor(SystemTypes.Reactor);
                }

                var oxygen2 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (oxygen2.IsActive)
                {
                    FixOxygen();
                }

                var lights2 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights2.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                break;
            case 2:
                var comms3 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (comms3.IsActive)
                {
                    FixComms();
                }

                var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                if (seismic.IsActive)
                {
                    FixReactor(SystemTypes.Laboratory);
                }

                var lights3 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights3.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                break;
            case 4:
                var comms4 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (comms4.IsActive)
                {
                    FixComms();
                }

                var reactor = ShipStatus.Instance.Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                if (reactor.IsActive)
                {
                    FixAirshipReactor();
                }

                var lights4 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights4.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                break;
            case 5:
                var reactor7 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactor7.IsActive)
                {
                    FixReactor(SystemTypes.Reactor);
                }

                var comms7 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                if (comms7.IsActive)
                {
                    FixMiraComms();
                }

                var mushroom = ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage]
                    .Cast<MushroomMixupSabotageSystem>();
                if (mushroom.IsActive)
                {
                    RpcFix(engineer, 1);
                }

                break;
            case 6:
                var reactor5 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactor5.IsActive)
                {
                    FixReactor(SystemTypes.Reactor);
                }

                var lights5 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights5.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                var comms5 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (comms5.IsActive)
                {
                    FixComms();
                }

                foreach (var i in PlayerControl.LocalPlayer.myTasks)
                {
                    if (i.TaskType == ModCompatibility.RetrieveOxygenMask)
                    {
                        RpcFix(engineer, 2);
                    }
                }

                break;
            case 7:
                var comms6 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                if (comms6.IsActive)
                {
                    FixComms();
                }

                var reactor6 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                if (reactor6.IsActive)
                {
                    FixReactor(SystemTypes.Reactor);
                }

                var oxygen6 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                if (oxygen6.IsActive)
                {
                    FixOxygen();
                }

                var lights6 = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                if (lights6.IsActive)
                {
                    RpcFix(engineer, 0);
                }

                if (ShipStatus.Instance.Systems.TryGetValue(SystemTypes.Laboratory, out var seismic1) &&
                    seismic1.Cast<IActivatable>().IsActive)
                {
                    FixReactor(SystemTypes.Laboratory);
                }

                break;
        }
    }

    private static void FixComms()
    {
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 0);
        RpcEngineerEventFix(PlayerControl.LocalPlayer);
    }

    private static void FixMiraComms()
    {
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
        RpcEngineerEventFix(PlayerControl.LocalPlayer);
    }

    private static void FixAirshipReactor()
    {
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
        RpcEngineerEventFix(PlayerControl.LocalPlayer);
    }

    private static void FixReactor(SystemTypes system)
    {
        ShipStatus.Instance.RpcUpdateSystem(system, 16);
        RpcEngineerEventFix(PlayerControl.LocalPlayer);
    }

    private static void FixOxygen()
    {
        ShipStatus.Instance.RpcUpdateSystem(SystemTypes.LifeSupp, 16);
        RpcEngineerEventFix(PlayerControl.LocalPlayer);
    }

    [MethodRpc((uint)TownOfSushiRpc.EngineerFix, SendImmediately = true)]
    private static void RpcFix(PlayerControl engineer, byte type)
    {
        if (engineer.Data.Role is not EngineerTOSRole)
        {
            Logger<TownOfSushiPlugin>.Error("Invalid engineer");
            return;
        }

        if (type == 0)
        {
            var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
            lights.ActualSwitches = lights.ExpectedSwitches;
        }
        else if (type == 1)
        {
            var mushroom = ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage]
                .Cast<MushroomMixupSabotageSystem>();
            mushroom.currentSecondsUntilHeal = 0.1f;
        }
        else if (type == 2)
        {
            ModCompatibility.RepairOxygen();
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.EngineerFix, engineer);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    [MethodRpc((uint)TownOfSushiRpc.EngineerEventFix, SendImmediately = true)]
    public static void RpcEngineerEventFix(PlayerControl engi)
    {
        if (engi.Data.Role is not EngineerTOSRole)
        {
            Logger<TownOfSushiPlugin>.Error("Invalid engineer");
            return;
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.EngineerFix, engi);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}