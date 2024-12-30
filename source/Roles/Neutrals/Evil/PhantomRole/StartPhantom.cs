namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            if (GetRole<Phantom>(__instance).Caught) return;
            value = !__instance.inVent;
        }
    }
}