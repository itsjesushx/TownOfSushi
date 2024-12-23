namespace TownOfSushi.Roles.Crewmates.Special.HaunterMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Haunter)) return;

            var role = GetRole<Haunter>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}