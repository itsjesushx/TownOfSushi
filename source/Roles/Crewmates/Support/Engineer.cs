namespace TownOfSushi.Roles
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = () => "Fix sabotages and vent around the map";
            TaskText = () => "Vent around and fix sabotages";
            RoleInfo = $"The Engineer is able to vent every {CustomGameOptions.EngiVentCooldown} seconds around the map  for {CustomGameOptions.EngiVentDuration} seconds and fix sabotages. The Engineer can fix a maximum of " + CustomGameOptions.MaxFixes + " sabotages.";
            LoreText = "A brilliant technician, you specialize in repairing sabotaged systems and navigating the map's vents. As the Engineer, you have the unique ability to vent around the map, bypassing obstacles and gaining quick access to critical areas. Your technical expertise is invaluable in keeping the crew safe and maintaining the map integrity, all while staying one step ahead of the Impostors.";
            Color = ColorManager.Engineer;
            RoleType = RoleEnum.Engineer;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
            MaxUses = CustomGameOptions.MaxFixes;
        }
        public DateTime LastVent { get; set; }
        public float EngineerTimer(DateTime last, float timer)
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - last;
            var num = timer * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public float TimeRemaining;
        public int MaxUses;
        public TMPro.TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformFix
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Engineer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            if (!__instance.enabled) return false;
            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var system = Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system == null) return false;
            var sabActive = system.AnyActive;
            if (!sabActive) return false;
            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;
            role.MaxUses -= 1;
            StartRPC(CustomRPC.EngineerFix, PlayerControl.LocalPlayer.NetId);
            switch (VanillaOptions().currentNormalGameOptions.MapId)
            {
                case 0:
                case 3:
                    var comms1 = Ship().Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                    if (comms1.IsActive) return FixComms();
                    var reactor1 = Ship().Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor1.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen1 = Ship().Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen1.IsActive) return FixOxygen();
                    var lights1 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights1.IsActive) return FixLights(lights1);

                    break;
                case 1:
                    var comms2 = Ship().Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                    if (comms2.IsActive) return FixMiraComms();
                    var reactor2 = Ship().Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor2.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen2 = Ship().Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen2.IsActive) return FixOxygen();
                    var lights2 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights2.IsActive) return FixLights(lights2);
                    break;

                case 2:
                    var comms3 = Ship().Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                    if (comms3.IsActive) return FixComms();
                    var seismic = Ship().Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                    if (seismic.IsActive) return FixReactor(SystemTypes.Laboratory);
                    var lights3 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights3.IsActive) return FixLights(lights3);
                    break;
                case 4:
                    var comms4 = Ship().Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                    if (comms4.IsActive) return FixComms();
                    var reactor = Ship().Systems[SystemTypes.HeliSabotage].Cast<HeliSabotageSystem>();
                    if (reactor.IsActive) return FixAirshipReactor();
                    var lights4 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights4.IsActive) return FixLights(lights4);
                    break;
                case 5:
                    var reactor7 = Ship().Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor7.IsActive) return FixReactor(SystemTypes.Reactor);
                    var comms7 = Ship().Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                    if (comms7.IsActive) return FixMiraComms();
                    var mushroom = Ship().Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                    if (mushroom.IsActive)
                    {
                        mushroom.currentSecondsUntilHeal = 0.1f;
                        return false;
                    } 
                    break;
                case 6:
                    var reactor5 = Ship().Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor5.IsActive) return FixReactor(SystemTypes.Reactor);
                    var lights5 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights5.IsActive) return FixLights(lights5);
                    var comms5 = Ship().Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                    if (comms5.IsActive) return FixComms();
                    foreach (PlayerTask i in PlayerControl.LocalPlayer.myTasks)
                    {
                        if (i.TaskType == RetrieveOxygenMask)
                        {
                            return FixSubOxygen();
                        }
                    }
                    break;
                case 7:
                    var comms6 = Ship().Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                    if (comms6.IsActive) return FixComms();
                    var reactor6 = Ship().Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                    if (reactor6.IsActive) return FixReactor(SystemTypes.Reactor);
                    var oxygen6 = Ship().Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                    if (oxygen6.IsActive) return FixOxygen();
                    var lights6 = Ship().Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    if (lights6.IsActive) return FixLights(lights6);
                    break;
            }

            

            return false;
        }

        private static bool FixComms()
        {
            Ship().RpcUpdateSystem(SystemTypes.Comms, 0);
            return false;
        }

        private static bool FixMiraComms()
        {
            Ship().RpcUpdateSystem(SystemTypes.Comms, 16 | 0);
            Ship().RpcUpdateSystem(SystemTypes.Comms, 16 | 1);
            return false;
        }

        private static bool FixAirshipReactor()
        {
            Ship().RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 0);
            Ship().RpcUpdateSystem(SystemTypes.HeliSabotage, 16 | 1);
            return false;
        }

        private static bool FixReactor(SystemTypes system)
        {
            Ship().RpcUpdateSystem(system, 16);
            return false;
        }

        private static bool FixOxygen()
        {
            Ship().RpcUpdateSystem(SystemTypes.LifeSupp, 16);
            return false;
        }

        private static bool FixSubOxygen()
        {
            RepairOxygen();

            StartRPC(CustomRPC.SubmergedFixOxygen, PlayerControl.LocalPlayer.NetId);

            return false;
        }

        private static bool FixLights(SwitchSystem lights)
        {
            StartRPC(CustomRPC.FixLights);

            lights.ActualSwitches = lights.ExpectedSwitches;

            return false;
        }
    }
    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    public class VentButtonDoClick
    {
        public static bool Prefix(VentButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer)) return true;
            if (IsDead()) return true;
            if (!__instance.enabled) return true;
            if (__instance.isCoolingDown && !PlayerControl.LocalPlayer.inVent) return false;
            var role = GetRole<Engineer>(PlayerControl.LocalPlayer);
            role.TimeRemaining = CustomGameOptions.EngiVentDuration;
            role.LastVent = DateTime.UtcNow;
            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class EngineerButtonSprite
    {
        private static void UpdtateVentTimer(HudManager __instance, Engineer role)
        {
            var ventButton = __instance.ImpostorVentButton;
            if (ventButton.cooldownTimerText == null) ventButton.cooldownTimerText = Object.Instantiate(__instance.KillButton.cooldownTimerText, ventButton.transform);
            if (PlayerControl.LocalPlayer.inVent)
            {
                if (role.TimeRemaining <= 0)
                {
                    ventButton.DoClick();
                }
                ventButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.EngiVentDuration);
                role.TimeRemaining -= Time.deltaTime;
            }
            else
            {
                ventButton.SetCoolDown(role.EngineerTimer(role.LastVent, CustomGameOptions.EngiVentCooldown), CustomGameOptions.EngiVentCooldown);
            }
        }

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer)) return;

            var role = GetRole<Engineer>(PlayerControl.LocalPlayer);

            UpdtateVentTimer(__instance, role);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.MaxUses + "";
            }

            __instance.KillButton.SetCoolDown(0f, 10f);
            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (IsDead()) return;
            if (!Ship()) return;
            var system = Ship().Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();
            if (system == null) return;
            var sabActive = system.AnyActive;
            var renderer = __instance.KillButton.graphic;
            if (sabActive & role.ButtonUsable & __instance.KillButton.enabled && PlayerControl.LocalPlayer.moveable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
            role.UsesText.color = Palette.DisabledClear;
            role.UsesText.material.SetFloat("_Desat", 1f);
        }
    }
}