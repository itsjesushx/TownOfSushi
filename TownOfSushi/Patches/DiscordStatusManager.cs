using Discord;

namespace TownOfSushi.Patches;

[HarmonyPatch]
internal class DiscordStatusManager
{
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity activity)
    {
        activity.Details += $" TownOfSushi v" + TownOfSushi.VersionString + TownOfSushi.DevString;
    }
}