

namespace TownOfSushi.Roles.Neutral.Evil.VultureRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vulture);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = GetRole<Vulture>(PlayerControl.LocalPlayer);
            if (role.EatTimer() != 0f) return false;
            var flag2 = __instance.isCoolingDown;
            
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = PlayerById(playerId);
            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }
            Rpc(CustomRPC.VultureEat, PlayerControl.LocalPlayer.PlayerId, playerId);
            role.LastEaten = DateTime.UtcNow;
            Coroutines.Start(Coroutine.EatCoroutine(role.CurrentTarget, role));
            return false;
        }
    }
}