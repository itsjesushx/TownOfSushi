namespace TownOfSushi.Roles.Neutral.Killing.PlaguebearerRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
            if (role.InfectTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            if (role.InfectedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        KillDistance();
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact.FullCooldownReset)
            {
                role.LastInfected = DateTime.UtcNow;
                return false;
            }
            else if (interact.GaReset)
            {
                role.LastInfected = DateTime.UtcNow;
                role.LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.InfectCd);
                return false;
            }
            else if (interact.ZeroSecReset) return false;
            return false;
        }
    }
}