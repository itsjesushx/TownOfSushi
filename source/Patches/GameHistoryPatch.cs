namespace TownOfSushi.Patches 
{
    public class PlayerHistory
    {

        public PlayerControl player;
        public DateTime timeOfDeath;
        public CustomDeathReason Reason;
        public PlayerControl GetKiller;
        public PlayerHistory(PlayerControl player, DateTime timeOfDeath, CustomDeathReason Reason, PlayerControl GetKiller) 
        {
            this.player = player;
            this.timeOfDeath = timeOfDeath;
            this.Reason = Reason;
            this.GetKiller = GetKiller;
        }

    }

    static class GameHistory 
    {
        public static List<PlayerHistory> deadPlayers = new List<PlayerHistory>();

        public static void CreateDeathReason(PlayerControl player, CustomDeathReason Reason, PlayerControl killer = null) 
        {
            var target = deadPlayers.FirstOrDefault(x => x.player.PlayerId == player.PlayerId);
            if (target != null) 
            {
                target.Reason = Reason;
                if (killer != null) 
                {
                    target.GetKiller = killer;
                }
            } 
            else if (player != null) 
            {  // Create dead player if needed:
                var dp = new PlayerHistory(player, DateTime.UtcNow, Reason, killer);
                deadPlayers.Add(dp);
            }
        }
    }
}