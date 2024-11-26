


using AmongUs.GameOptions;

namespace TownOfSushi.Roles.Neutral.Killing.JuggernautRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = GetRole<Juggernaut>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.KillTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        KillDistance();
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact.AbilityUsed) return false;
            else if (interact.FullCooldownReset)
            {
                role.LastKill = DateTime.UtcNow;
                return false;
            }
            else if (interact.GaReset)
            {
                role.LastKill = DateTime.UtcNow;
                role.LastKill = role.LastKill.AddSeconds(-(CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills) + CustomGameOptions.ProtectKCReset);
                return false;
            }
            else if (interact.ZeroSecReset) return false;
            return false;
        }
    }
}