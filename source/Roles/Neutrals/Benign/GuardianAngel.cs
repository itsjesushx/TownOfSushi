using TMPro;

namespace TownOfSushi.Roles
{
    public class GuardianAngel : Role
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int MaxUses;
        public TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
        public PlayerControl target;
        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = () =>
                target == null ? "You don't have a target" : $"Protect {target.name} with your life!";
            TaskText = () =>
                target == null
                    ? "You don't have a target"
                    : $"Protect {target.name}!";
            RoleInfo = $"The Guardian Angel can protect a player from death. They have a limited number of uses. Protecting a player makes them be unable to die for a {CustomGameOptions.ProtectDuration} seconds. If the target gets a murder attempt, the killer will have a {CustomGameOptions.ProtectKCReset} seconds cooldown reset.";
            LoreText = "A celestial protector, you watch over the alive with unwavering devotion. As the Guardian Angel, you are tasked with protecting a specific player, shielding them from harm. Your divine abilities allow you to intervene in their time of need, using your powers to prevent death and ensure their survival. but your protection is limited, and once it is used up and the game ends, your mission ends.";
            Color = ColorManager.GuardianAngel;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
            MaxUses = CustomGameOptions.MaxProtects;
        }
        public bool Protecting => TimeRemaining > 0f;
        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }
        public void UnProtect()
        {
            var ga = GetRole<GuardianAngel>(Player);
            if (!ga.target.IsShielded())
            {
                ga.target.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                ga.target.myRend().material.SetFloat("_Outline", 0f);
            }
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudManagerGuardianAngel
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateProtectButton(__instance);
        }

        public static void UpdateProtectButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel)) return;
            var protectButton = __instance.KillButton;

            var role = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(protectButton.cooldownTimerText, protectButton.transform);
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

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.Protecting) protectButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.ProtectDuration);
            else if (role.ButtonUsable) protectButton.SetCoolDown(role.ProtectTimer(), CustomGameOptions.ProtectCd);
            else protectButton.SetCoolDown(0f, CustomGameOptions.ProtectCd);
           protectButton.transform.localPosition = new Vector3(-2f, -0.06f, 0);

            var renderer = protectButton.graphic;
            if (role.Protecting || (!protectButton.isCoolingDown && role.ButtonUsable))
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

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformProtect
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            var role = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
            if (!role.ButtonUsable) return false;
            var protectButton = HUDManager().KillButton;
            if (__instance == protectButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.ProtectTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                role.TimeRemaining = CustomGameOptions.ProtectDuration;
                role.MaxUses--;
                role.Protect();
                SoundEffectsManager.Play("trapperTrap");
                StartRPC(CustomRPC.GAProtect, PlayerControl.LocalPlayer.PlayerId);
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ProtectUnportect
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;
                if (ga.Protecting)
                    ga.Protect();
                else if (ga.Enabled) ga.UnProtect();
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowProtect
    {
        public static Color ProtectedColor = new Color(1f, 0.85f, 0f, 1f);
        public static Color ShieldedColor = Color.cyan;
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;

                var player = ga.target;
                if (player == null) continue;

                if ((player.Data.IsDead || ga.Player.Data.IsDead || ga.Player.Data.Disconnected) && !player.IsShielded())
                {
                    player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    player.myRend().material.SetFloat("_Outline", 0f);
                    continue;
                }
            }
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GATargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, GuardianAngel role)
        {
            if (CustomGameOptions.GAKnowsTargetRole) return;
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.text += "<color=#FFFFFFFF> [★]</color>";
        }

        private static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel)) return;
            if (IsDead()) return;

            var role = GetRole<GuardianAngel>(PlayerControl.LocalPlayer);

            if (Meeting() != null) UpdateMeeting(Meeting(), role);

            if (!CustomGameOptions.GAKnowsTargetRole && !CamouflageUnCamouflagePatch.IsCamouflaged) role.target.nameText().text += "<color=#FFFFFFFF> ★</color>";

            if (role.target == null || (!role.target.Data.IsDead && !role.target.Data.Disconnected)) return;

            StartRPC(CustomRPC.GuardianAngelChangeRole, PlayerControl.LocalPlayer.PlayerId);

            Object.Destroy(role.UsesText);
            HUDManager().KillButton.gameObject.SetActive(false);

            GuardianAngelChangeRole(PlayerControl.LocalPlayer);
        }

        public static void GuardianAngelChangeRole(PlayerControl player)
        {
            var ga = GetRole<GuardianAngel>(player);
            player.myTasks.RemoveAt(0);
            RoleDictionary.Remove(player.PlayerId);

            if (CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.ReDoTaskText();
            }
            else if (CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.ReDoTaskText();
            }
            else
            {
                new Crewmate(player);
            }
        }
    }
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class GuardianAngelMeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var GARole = (GuardianAngel)role;
                if (player.PlayerId == GARole.target.PlayerId && CustomGameOptions.GADiesWithClient)
                {
                    if (!GARole.Player.Data.IsDead)
                    {
                        role.Player.Exiled();
                    }
                }
            }
        }
    }
}