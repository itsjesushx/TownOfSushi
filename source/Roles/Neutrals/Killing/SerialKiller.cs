namespace TownOfSushi.Roles
{
    public class SerialKiller : Role
    {
        private KillButton _stabButton;
        public bool Enabled;
        public PlayerControl ClosestPlayer;
        public DateTime LastStabbed;
        public DateTime LastKilled;
        public float TimeRemaining;
        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = () => "Stab to kill everyone";
            TaskText = () => "Kill everyone in stabbing mode";
            RoleInfo = "The Serial Killer is a Neutral role with its own win condition. Although the Serial Killer has a kill button, they can't use it unless they are stabbing. Once the Serial Killer rampages they gain Impostor vision and the ability to kill. However, unlike most killers their kill cooldown is really short. The Serial Killer needs to be the last killer alive to win the game.";
            LoreText = "You are the Serial Killer, a cold-blooded predator with a single goal—eliminate everyone in your path. Armed with your trusty knife, you move through the shadows, striking at your victims without hesitation. Your mind is focused, your mission clear: kill them all. As the last remaining survivor, you'll be free from the chaos that surrounds you. Trust no one, for every crewmate is a potential target, and every moment could be your next deadly strike.";
            Color = Colors.SerialKiller;
            LastStabbed = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.SerialKiller;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public KillButton StabButton
        {
            get => _stabButton;
            set
            {
                _stabButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public bool Stabbing => TimeRemaining > 0f;
        public float StabTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStabbed;
            var num = CustomGameOptions.StabCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Stab()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void UnStab()
        {
            Enabled = false;
            LastStabbed = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.StabKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class StabUnStab
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller) role;
                if (sk.Stabbing)
                    sk.Stab();
                else if (sk.Enabled) sk.UnStab();
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformStab
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = GetRole<SerialKiller>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;

            if (__instance == role.StabButton)
            {
                if (role.StabTimer() != 0) return false;
                var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
                if (!abilityUsed) return false;
                if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;

                role.TimeRemaining = CustomGameOptions.Stabeduration;
                role.Stab();
                return false;
            }

            if (role.KillTimer() != 0) return false;
            if (!role.Stabbing) return false;
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        KillDistance();
            if (!flag3) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[3] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.StabKillCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudStab
    {
        public static Sprite StabSprite => TownOfSushi.StabSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) return;
            var role = GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.StabKillCd);

            if (role.StabButton == null)
            {
                role.StabButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StabButton.graphic.enabled = true;
                role.StabButton.gameObject.SetActive(false);
            }

            role.StabButton.graphic.sprite = StabSprite;
            role.StabButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            role.StabButton.buttonLabelText.gameObject.SetActive(true);
            role.StabButton.buttonLabelText.text = "STAB";

            role.StabButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stabbing)
            {
                role.StabButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.Stabeduration);
                role.StabButton.graphic.color = Palette.EnabledColor;
                role.StabButton.graphic.material.SetFloat("_Desat", 0f);
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else SetTarget(ref role.ClosestPlayer, __instance.KillButton);

                return;
            }
            else
            {
                role.StabButton.SetCoolDown(role.StabTimer(), CustomGameOptions.StabCd);

                if (role.StabTimer() > 0f || !PlayerControl.LocalPlayer.moveable)
                {
                    role.StabButton.graphic.color = Palette.DisabledClear;
                    role.StabButton.graphic.material.SetFloat("_Desat", 1f);
                }
                else
                {
                    role.StabButton.graphic.color = Palette.EnabledColor;
                    role.StabButton.graphic.material.SetFloat("_Desat", 0f);
                }

                return;
            }
        }
    }
}