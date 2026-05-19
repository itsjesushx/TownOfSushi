using System.Collections.Generic;

namespace TownOfSushi.Roles
{
    public class Amnesiac
    {
        public static readonly Dictionary<byte, Amnesiac> Amnesiacs = new Dictionary<byte, Amnesiac>();
        public List<byte> PlayersToRemember = new List<byte>();
        public static Color Color = new Color32(138, 189, 255, byte.MaxValue);
        public bool Remembered;
        public readonly PlayerControl amnesiac;
        public Amnesiac(PlayerControl player)
        {
            amnesiac = player;
            Amnesiacs.Add(player.PlayerId, this);
            PlayersToRemember = new List<byte>();
            Remembered = false;
            GameHistory.AddToRoleHistory(player.PlayerId, Role.amnesiac);
        }
        public static void RemoveAmnesiac(byte playerId)
        {
            Amnesiacs.Remove(playerId);
        }
        public static bool IsAmnesiac(byte playerId, out Amnesiac Amnesiac)
        {
            return Amnesiacs.TryGetValue(playerId, out Amnesiac);
        }

        public static void ClearAndReload()
        {
            Amnesiacs.Clear();
        }
    }
}