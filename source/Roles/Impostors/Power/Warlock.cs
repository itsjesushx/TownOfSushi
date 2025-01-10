namespace TownOfSushi.Roles
{
    public class Warlock : Role
    {
        public Warlock(PlayerControl player) : base(player)
        {
            Name = "Warlock";
            StartText = () => "Charge Up Your Kill Button To Multi Kill";
            TaskText = () => "Kill people in small bursts";
            RoleInfo = "As the warlock, you have to wait until your charger gets to 100% to kill, you can kill anyone that does not have protection with your kill button, just like a serial killer on bloodlust. The difference is that warlock kills faster.";
            LoreText = "A dark sorcerer, you harness the power of dark magic to strike fear into the hearts of the crew. As the Warlock, you can charge up your kill ability, allowing you to unleash a devastating multi-kill in short bursts. The more you charge, the deadlier your strikes become, giving you the power to eliminate multiple targets at once and wreak havoc on the crew.";
            Color = Colors.Impostor;
            RoleType = RoleEnum.Warlock;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
            ChargePercent = 0;
        }
        public TMPro.TextMeshPro ChargeText;
        public int ChargePercent;
        public bool Charging;
        public bool UsingCharge;
        public float ChargeUseDuration;
        public DateTime StartChargeTime;
        public DateTime StartUseTime;
        public int ChargeUpTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartChargeTime;
            var num = CustomGameOptions.ChargeUpDuration * 1000f;
            var result = (float)timeSpan.TotalMilliseconds/num * 100f;
            if (result > 100f) result = 100f;
            return Convert.ToInt32(Math.Round(result));
        }

        public int ChargeUseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = StartUseTime - utcNow;
            var num = ChargeUseDuration * 1000f;
            var result = ((float)timeSpan.TotalMilliseconds / num + 1) * ChargeUseDuration / CustomGameOptions.ChargeUseDuration * 100f;
            if (result < 0f) result = 0f;
            return Convert.ToInt32(Math.Round(result));
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdateWarlock
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warlock)) return;
            var role = GetRole<Warlock>(PlayerControl.LocalPlayer);

            if (role.ChargeText == null)
            {
                role.ChargeText = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.ChargeText.gameObject.SetActive(false);
                role.ChargeText.transform.localPosition = new Vector3(
                    role.ChargeText.transform.localPosition.x + 0.26f,
                    role.ChargeText.transform.localPosition.y + 0.29f,
                    role.ChargeText.transform.localPosition.z);
                role.ChargeText.transform.localScale = role.ChargeText.transform.localScale * 0.65f;
                role.ChargeText.alignment = TMPro.TextAlignmentOptions.Right;
                role.ChargeText.fontStyle = TMPro.FontStyles.Bold;
                role.ChargeText.enableWordWrapping = false;
            }
            if (role.ChargeText != null)
            {
                role.ChargeText.text = role.ChargePercent + "%";
            }
            role.ChargeText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started
                    && (role.Charging || role.UsingCharge));
            if (role.UsingCharge)
            {
                if (role.Charging)
                {
                    role.StartUseTime = DateTime.UtcNow;
                    role.Charging = false;
                }
            }
            else if (PlayerControl.LocalPlayer.killTimer == 0f)
            {
                if (!role.Charging)
                {
                    role.StartChargeTime = DateTime.UtcNow;
                    role.Charging = true;
                }
            }
            else
            {
                role.ChargePercent = 0;
                role.Charging = false;
                role.UsingCharge = false;
            }
            return;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ChargeUnCharge
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Warlock)) return;
            foreach (var role in GetRoles(RoleEnum.Warlock))
            {
                var warlock = (Warlock) role;
                if (warlock.Charging)
                    warlock.ChargePercent = warlock.ChargeUpTimer();
                else if (warlock.UsingCharge)
                {
                    warlock.ChargePercent = warlock.ChargeUseTimer();
                    if (warlock.ChargePercent <= 0f)
                    {
                        warlock.UsingCharge = false;
                        if (warlock.Player.Is(ModifierEnum.Underdog))
                        {
                            var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                            var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                            var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                            warlock.Player.SetKillTimer(UnderdogPerformKill.LastImp() ? lowerKC : (UnderdogPerformKill.IncreasedKC() ? normalKC : upperKC));
                        }
                        else warlock.Player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    }
                }
            }
        }
    }
}