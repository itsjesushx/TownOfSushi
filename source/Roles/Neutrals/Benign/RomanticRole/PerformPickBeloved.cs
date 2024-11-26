namespace TownOfSushi.Roles.Neutral.Benign.RomanticRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Romantic);
            if (!flag) return true;
            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.PickTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.AlreadyPicked) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact.AbilityUsed)
            {
                role.Beloved = role.ClosestPlayer;
                role.AlreadyPicked = true;
                Rpc(CustomRPC.SetRomanticTarget, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact.FullCooldownReset)
            {
                role.LastPick = DateTime.UtcNow;
                return false;
            }
            else if (interact.GaReset)
            {
                role.LastPick = DateTime.UtcNow;
                role.LastPick = role.LastPick.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.PickStartTimer);
                return false;
            }
            else if (interact.ZeroSecReset) return false;
            return false;
        }
    }
}