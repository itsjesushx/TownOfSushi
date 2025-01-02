namespace TownOfSushi.Roles
{
    public class SerialKiller : Role
    {
        private KillButton _rampageButton;
        public bool Enabled;
        public PlayerControl ClosestPlayer;
        public DateTime LastStabbed;
        public DateTime LastKilled;
        public float TimeRemaining;
        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = () => "Stab to kill everyone";
            TaskText = () => "Stab to kill everyone";
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
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public bool Stabbed => TimeRemaining > 0f;
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

        public void Unrampage()
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
    public class StabUnrampage
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.SerialKiller))
            {
                var werewolf = (SerialKiller) role;
                if (werewolf.Stabbed)
                    werewolf.Stab();
                else if (werewolf.Enabled) werewolf.Unrampage();
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
            if (!role.Stabbed) return false;
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

            role.StabButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stabbed)
            {
                role.StabButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.Stabeduration);
                role.StabButton.graphic.color = Palette.EnabledColor;
                role.StabButton.graphic.material.SetFloat("_Desat", 0f);
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

                return;
            }
            else
            {
                role.StabButton.SetCoolDown(role.StabTimer(), CustomGameOptions.StabCd);

                role.StabButton.graphic.color = Palette.EnabledColor;
                role.StabButton.graphic.material.SetFloat("_Desat", 0f);

                return;
            }
        }
    }
}