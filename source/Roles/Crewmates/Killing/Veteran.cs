namespace TownOfSushi.Roles
{
    public class Veteran : Role
    {
        public bool Enabled;
        public DateTime LastAlerted;
        public float TimeRemaining;
        public int UsesLeft;
        public TMPro.TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public Veteran(PlayerControl player) : base(player)
        {
            Name = "Veteran";
            StartText = () => "Alert to kill anyone who interacts with you";
            TaskText = () => "Alert to kill whoever interacts with you";
            RoleInfo = "The Veteran is able to alert, Alerting makes the Veteran Unkillable and will kill anyone who interacts with them except for the Pestilence. The Veteran dies if a Pestilence attempts to murder them even if they are on alert. The Veteran can alert a maximum of " + CustomGameOptions.MaxAlerts + " times.";
            LoreText = "A seasoned and battle-hardened defender, you are always on high alert. As the Veteran, you can trigger an alert to eliminate anyone who dares to interact with you. Your instincts are sharp, and your experience makes you a dangerous force—keeping the killers at bay with deadly precision.";
            Color = Colors.Veteran;
            LastAlerted = DateTime.UtcNow;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Veteran;
            UsesLeft = CustomGameOptions.MaxAlerts;
        }
        public bool OnAlert => TimeRemaining > 0f;
        public float AlertTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAlerted;
            ;
            var num = CustomGameOptions.AlertCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Alert()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }


        public void UnAlert()
        {
            Enabled = false;
            LastAlerted = DateTime.UtcNow;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class AlertUnalert
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran) role;
                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled) veteran.UnAlert();
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PeroformAlert
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Veteran);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Veteran>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var alertButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == alertButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.AlertTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemaining = CustomGameOptions.AlertDuration;
                role.UsesLeft--;
                role.Alert();
                Rpc(CustomRPC.Alert, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudAlert
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateAlertButton(__instance);
        }

        public static void UpdateAlertButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Veteran)) return;
            var alertButton = __instance.KillButton;

            var role = GetRole<Veteran>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(alertButton.cooldownTimerText, alertButton.transform);
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
                role.UsesText.text = role.UsesLeft + "";
            }

            alertButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.OnAlert) alertButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.AlertDuration);
            else if (role.ButtonUsable) alertButton.SetCoolDown(role.AlertTimer(), CustomGameOptions.AlertCd);
            else alertButton.SetCoolDown(0f, CustomGameOptions.AlertCd);

            var renderer = alertButton.graphic;
            if (role.OnAlert || (!alertButton.isCoolingDown && role.ButtonUsable))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}