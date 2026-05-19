using System.Collections.Generic;

namespace TownOfSushi.Roles
{
    public class Jester
    {
        public static readonly Dictionary<byte, Jester> Jesters = new Dictionary<byte, Jester>();
        public static Color Color = new Color32(255, 191, 204, byte.MaxValue);
        public PlayerControl CurrentTarget;
        public bool HasKilled = false;
        public static PlayerControl WinningJesterPlayer = null;
        public static bool IsJesterWin = false;
        public readonly PlayerControl jester;
        public Jester(PlayerControl player)
        {
            jester = player;
            HasKilled = false;
            Jesters.Add(player.PlayerId, this);
            GameHistory.AddToRoleHistory(player.PlayerId, Role.jester);
        }

        public static bool IsJester(byte playerId, out Jester jester)
        {
            return Jesters.TryGetValue(playerId, out jester);
        }
        public static void RemoveJester(byte playerId)
        {
            Jesters.Remove(playerId);
        }
        public static void ClearAndReload()
        {
            Jesters.Clear();
            IsJesterWin = false;
            WinningJesterPlayer = null;
        }
    }
}