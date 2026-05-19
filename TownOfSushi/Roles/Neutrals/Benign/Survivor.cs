using System.Collections.Generic;

namespace TownOfSushi.Roles
{
    public class Survivor
    {
        public static readonly Dictionary<byte, Survivor> Survivors = new Dictionary<byte, Survivor>();
        public readonly PlayerControl survivor;
        public Survivor(PlayerControl player)
        {
            survivor = player;
            target = null;
            Survivors.Add(player.PlayerId, this);
            blankedList = new List<PlayerControl>();
            blanks = 0;
            GameHistory.AddToRoleHistory(player.PlayerId, Role.survivor);
        }
        public bool SafeguardActive = false;
        public PlayerControl target;
        public static List<PlayerControl> blankedList = new List<PlayerControl>();
        public static Color Color = new Color32(255, 227, 105, byte.MaxValue);
        public int blanks = 0;
        public static bool IsSurvivor(byte playerId, out Survivor Survivor)
        {
            return Survivors.TryGetValue(playerId, out Survivor);
        }
        public static void RemoveSurvivor(byte playerId)
        {
            Survivors.Remove(playerId);
        }

        public static void ClearAndReload()
        {
            Survivors.Clear();
        }
    }
}