using Discord;
using HarmonyLib;
using TownOfSushi;

public static class DiscordStatusManager
{
    [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
    [HarmonyPrefix]
    public static void Prefix([HarmonyArgument(0)] Activity Activity)
    {
        Activity.Details += $" TownOfSushi v" + TownOfSushiPlugin.Version.ToString();
    }
}