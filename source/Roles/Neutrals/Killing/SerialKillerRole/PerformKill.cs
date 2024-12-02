namespace TownOfSushi.Roles.Neutral.Killing.SerialKillerRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
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
            if (interact[4] == true) return false;
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
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
