using System.Collections.Generic;
using UnityEngine;

namespace TownOfSushi{
    static class MapOptions 
    {
        // Set values
        public static int maxNumberOfMeetings = 10;
        public static bool blockSkippingInEmergencyMeetings = false;
        public static bool noVoteIsSelfVote = false;
        public static bool hidePlayerNames = false;
        public static bool ghostsSeeRoles = true;
        public static bool ghostsSeeModifier = true;
        public static bool ghostsSeeInformation = true;
        public static bool ghostsSeeVotes = true;
        public static bool showRoleSummary = true;
        public static bool allowParallelMedBayScans = false;
        public static bool showLighterDarker = true;
        public static bool enableSoundEffects = true;
        public static bool enableHorseMode = false;
        public static bool shieldFirstKill = false;
        public static bool ShowVentsOnMap = true;
        public static bool ShowChatNotifications = true;

        // Updating values
        public static int meetingsCount = 0;
        public static List<SurvCamera> camerasToAdd = new List<SurvCamera>();
        public static List<Vent> ventsToSeal = new List<Vent>();
        public static Dictionary<byte, PoolablePlayer> playerIcons = new Dictionary<byte, PoolablePlayer>();
        public static string firstKillName;
        public static PlayerControl firstKillPlayer;

        public static void ClearAndReloadMapOptions() 
        {
            meetingsCount = 0;
            camerasToAdd = new List<SurvCamera>();
            ventsToSeal = new List<Vent>();
            playerIcons = new Dictionary<byte, PoolablePlayer>(); ;

            maxNumberOfMeetings = Mathf.RoundToInt(CustomOptionHolder.maxNumberOfMeetings.GetSelection());
            blockSkippingInEmergencyMeetings = CustomOptionHolder.blockSkippingInEmergencyMeetings.GetBool();
            noVoteIsSelfVote = CustomOptionHolder.noVoteIsSelfVote.GetBool();
            hidePlayerNames = CustomOptionHolder.hidePlayerNames.GetBool();
            allowParallelMedBayScans = CustomOptionHolder.allowParallelMedBayScans.GetBool();
            shieldFirstKill = CustomOptionHolder.shieldFirstKill.GetBool();
            firstKillPlayer = null;
        }

        public static void reloadPluginOptions() {
            ghostsSeeRoles = TownOfSushiPlugin.GhostsSeeRoles.Value;
            ghostsSeeModifier = TownOfSushiPlugin.GhostsSeeModifier.Value;
            ghostsSeeInformation = TownOfSushiPlugin.GhostsSeeInformation.Value;
            ghostsSeeVotes = TownOfSushiPlugin.GhostsSeeVotes.Value;
            showRoleSummary = TownOfSushiPlugin.ShowRoleSummary.Value;
            showLighterDarker = TownOfSushiPlugin.ShowLighterDarker.Value;
            enableSoundEffects = TownOfSushiPlugin.EnableSoundEffects.Value;
            enableHorseMode = TownOfSushiPlugin.EnableHorseMode.Value;
            ShowVentsOnMap = TownOfSushiPlugin.ShowVentsOnMap.Value;
            ShowChatNotifications = TownOfSushiPlugin.ShowChatNotifications.Value;

            //Patches.ShouldAlwaysHorseAround.isHorseMode = TownOfSushiPlugin.EnableHorseMode.Value;
        }
    }
}
