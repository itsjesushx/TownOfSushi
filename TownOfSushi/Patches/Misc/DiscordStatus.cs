using Discord;
using HarmonyLib;

namespace TownOfSushi.Patches.Misc;

[HarmonyPatch]
public static class DiscordStatus
{
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity activity)
    {
        activity.Details += $" Town of Sushi v{TownOfSushiPlugin.Version}{TownOfSushiPlugin.DevString}";
    }
}