namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public class VitalsMinigameUpdatePatch
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (IsDead()) return true;
            if (LocalPlayer().Is(RoleEnum.Medium) && !CustomGameOptions.MediumVitals)
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }
            return true;
        }
    }
}