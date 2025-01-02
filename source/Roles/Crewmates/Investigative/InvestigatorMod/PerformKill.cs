using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return true;
            var role = GetRole<Investigator>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();

            if (__instance == role.ExamineButton)
            {
                var flag2 = role.ExamineTimer() == 0f;
                if (!flag2) return false;
                if (!role.ExamineMode) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[3] == true)
                {
                    if (role.ClosestPlayer == role.DetectedKiller) Flash(Color.red);
                    else Flash(Color.green);
                }
                if (interact[0] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                    return false;
                }
                else if (interact[2] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                var playerId = role.CurrentTarget.ParentId;
                var player = PlayerById(playerId);
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId)
                    {
                        role.DetectedKiller = PlayerById(deadPlayer.KillerId);
                        role.ExamineMode = true;
                    }
                }
                return false;
            }
        }
    }
}
