namespace TownOfSushi.Roles.Neutral.Killing.GlitchRole
{
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    internal class EngineerMapOpen
    {
        private static bool Prefix(MapBehaviour __instance)
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch);
        }
    }
}