namespace TownOfSushi.Roles
{
    public static class Lawyer 
    {
        public static PlayerControl Player;
        public static PlayerControl Target;
        public static Color Color = new Color32(134, 153, 25, byte.MaxValue);
        public static bool TriggerLawyerWin = false;
        public static int Meetings = 0;
        public static bool WinsAfterMeetings = false;
        public static void ClearAndReload() 
        {
            Player = null;
            Target = null;
            TriggerLawyerWin = false;
            Meetings = 0;
            WinsAfterMeetings = CustomGameOptions.LawyerWinsAfterMeetings;
        }
    }
}