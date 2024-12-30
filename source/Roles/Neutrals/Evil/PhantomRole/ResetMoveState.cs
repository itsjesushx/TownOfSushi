namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Phantom)) return;

            var role = GetRole<Phantom>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}