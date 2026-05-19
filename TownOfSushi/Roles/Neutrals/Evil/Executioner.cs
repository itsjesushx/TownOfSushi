namespace TownOfSushi.Roles
{
    public static class Executioner
    {
        public static PlayerControl Player;
        public static PlayerControl target;
        public static Color Color = new Color32(166, 131, 212, byte.MaxValue);
        
        public static bool IsExecutionerWin = false;

        public static void ClearAndReload(bool clearTarget = true) 
        {
            Player = null;
            if (clearTarget) 
            {
                target = null;
            }
            IsExecutionerWin = false;
        }
    }
}