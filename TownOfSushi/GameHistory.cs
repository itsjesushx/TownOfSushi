using System.Collections.Generic;
using System.Linq;

namespace TownOfSushi
{
    public static class GameHistory
    {
        public static List<Tuple<Vector3, bool>> LocalPlayerPositions = new();
        public static List<DeadPlayer> DeadPlayers = new();
        public static readonly Dictionary<byte, List<Role>> RoleHistory = new();
        public static Dictionary<byte, List<KillListTypes>> KillList = new();
        public static void ClearGameHistory()
        {
            LocalPlayerPositions.Clear();
            DeadPlayers.Clear();
            RoleHistory.Clear();
            KillList.Clear();
        }

        public static void AddToRoleHistory(byte playerId, Role role)
        {
            if (!RoleHistory.ContainsKey(playerId))
                RoleHistory[playerId] = new List<Role>();

            if (!RoleHistory[playerId].Contains(role))
                RoleHistory[playerId].Add(role);
        }

        public static void AddToKillList(byte playerId, KillListTypes type)
        {
            if (!KillList.ContainsKey(playerId))
                KillList[playerId] = new List<KillListTypes>();

            KillList[playerId].Add(type);
        }

        public static void CreateDeathReason(
            PlayerControl player,
            DeadPlayer.CustomDeathReason deathReason,
            PlayerControl killer = null)
        {
            if (player == null) return;

            var existing = DeadPlayers
                .FirstOrDefault(x => x.player.PlayerId == player.PlayerId);

            if (existing != null)
            {
                existing.DeathReason = deathReason;

                if (killer != null)
                    existing.GetKiller = killer;
            }
            else
            {
                var dp = new DeadPlayer(player, DateTime.UtcNow, deathReason, killer);
                DeadPlayers.Add(dp);
            }
        }
    }
}
