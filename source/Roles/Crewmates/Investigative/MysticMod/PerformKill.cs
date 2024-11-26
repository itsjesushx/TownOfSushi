using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Crewmates.Investigative.MysticMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return true;
            var role = GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ExamineTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact.AbilityUsed)
            {
                var hasKilled = false;
                foreach (var player in Murder.KilledPlayers)
                {
                    if (player.KillerId == role.ClosestPlayer.PlayerId && (float)(DateTime.UtcNow - player.KillTime).TotalSeconds < CustomGameOptions.RecentKill)
                    {
                        hasKilled = true;
                    }
                }
                if (hasKilled) Flash(Color.red);
                else Flash(Color.green);
                role.LastExaminedPlayer = role.ClosestPlayer;
            }
            if (interact.FullCooldownReset == true)
            {
                role.LastExamined = DateTime.UtcNow;
                return false;
            }
            else if (interact.GaReset == true)
            {
                role.LastExamined = DateTime.UtcNow;
                role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.MysticExamineCd);
                return false;
            }
            else if (interact.ZeroSecReset == true) return false;
            return false;
        }
    }
}
