namespace TownOfSushi.Roles.Crewmates.Support.GuardianMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Guardian);
            if (!flag) return true;
            var role = GetRole<Guardian>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.ProtectTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Target = role.ClosestPlayer;
                Rpc(CustomRPC.VoteProtect, PlayerControl.LocalPlayer.PlayerId, role.Target.PlayerId);
                
            }
            if (interact[0] == true)
            {
                role.LastProtect = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastProtect = DateTime.UtcNow;
                role.LastProtect = role.LastProtect.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.VoteProtectCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
