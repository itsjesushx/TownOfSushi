using HarmonyLib;
using Discord;
using MiraAPI;
using UnityEngine;
using Reactor.Utilities;

namespace TownOfSushi.Patches.Misc
{
    [HarmonyPatch]
    public static class DiscordStatus
    {
        [HarmonyPatch(typeof(ActivityManager), nameof(ActivityManager.UpdateActivity))]
        [HarmonyPrefix]
        public static void Prefix([HarmonyArgument(0)] Activity activity)
        {
            string details = $"TownOfSushi v{TownOfSushiPlugin.Version} {TownOfSushiPlugin.DevString}";

            try
            {
                if (activity.State == "In Menus")
                {
                    int maxPlayers = GameOptionsManager.Instance.currentNormalGameOptions.MaxPlayers;
                    var lobbyCode = GameStartManager.Instance.GameRoomNameCode.text;
                    var miraAPIVersion = MiraApiPlugin.Version;
                    var platform = Application.platform;

                    details += $" Players: {maxPlayers} | Lobby Code: {lobbyCode} | MiraAPI Version {miraAPIVersion} | Platform: {platform}";
                }

                else if (activity.State == "In Game" && MeetingHud.Instance)
                {
                    details += " | \nIn Meeting";
                }

                activity.Details = details;
                activity.Assets.SmallText = "TownOfSushi Created With MiraAPI";
            }
            catch (System.Exception e)
            {
                Logger<TownOfSushiPlugin>.Error($"Error updating Discord activity: {e.Message}\nStackTrace: {e.StackTrace}");
            }
        }
    }
}