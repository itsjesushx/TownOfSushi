namespace TownOfSushi.Roles.Neutral.Killing.GlitchRole
{
    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    public class StopReport
    {
        [HarmonyPriority(Priority.First)]
        public static bool Prefix(ReportButton __instance)
        {
            if (PlayerControl.LocalPlayer.IsHacked())
            {
                Coroutines.Start(Glitch.AbilityCoroutine.Hack(PlayerControl.LocalPlayer));
                return false;
            }
            return true;
        }
    }
}
