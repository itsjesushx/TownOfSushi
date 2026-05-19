using System.Collections.Generic;

namespace TownOfSushi
{
    static class MapOptions 
    {

        // Updating values
        public static int meetingsCount = 0;
        public static List<SurvCamera> CamsToAdd = new List<SurvCamera>();
        public static List<Vent> VentsToSeal = new List<Vent>();
        public static List<byte> RevivedPlayers = new List<byte>();
        public static Dictionary<byte, PoolablePlayer> BeanIcons = new Dictionary<byte, PoolablePlayer>();
        public static string FirstKillName;
        public static PlayerControl FirstPlayerKilled;
        public static bool IsFirstRound { get; set; } = true;
        public static void ClearAndReloadMapOptions() 
        {
            meetingsCount = 0;
            CamsToAdd = new List<SurvCamera>();
            VentsToSeal = new List<Vent>();
            BeanIcons = new Dictionary<byte, PoolablePlayer>();
            RevivedPlayers = new List<byte>();
            IsFirstRound = true;
            FirstPlayerKilled = null;
        }
    }
}
