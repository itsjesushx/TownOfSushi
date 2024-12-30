namespace TownOfSushi.Roles.Crewmates.Special.HaunterMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.Is(RoleEnum.Haunter)) amDead = GetRole<Haunter>(__instance.myPlayer).Caught;
        }
    }
}