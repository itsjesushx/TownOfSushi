namespace TownOfSushi.Roles.Crewmates.Support.MedicRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Protect
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Medic);
            if (!flag) return true;
            var role = GetRole<Medic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!__instance.enabled) return false;
            if (role.UsedAbility || role.ClosestPlayer == null) return false;
            if (role.StartTimer() > 0) return false;

            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact.AbilityUsed)
            {
                Rpc(CustomRPC.Protect, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);

                role.ShieldedPlayer = role.ClosestPlayer;
                role.UsedAbility = true;
                return false;
            }
            return false;
        }
    }
}
