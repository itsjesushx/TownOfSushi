namespace TownOfSushi.Roles.Neutral.Killing.HitmanRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    internal class PerformAbilities
    {
        public static bool Prefix(KillButton __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman) && __instance.isActiveAndEnabled &&
                !__instance.isCoolingDown && PlayerControl.LocalPlayer.CanMove && !PlayerControl.LocalPlayer.inVent)
                return GetRole<Hitman>(PlayerControl.LocalPlayer).UseAbility(__instance);

            return true;
        }
    }
}