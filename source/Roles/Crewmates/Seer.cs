namespace TownOfSushi.Roles.Crewmates
{
    public class Seer : Role
    {
        public List<byte> Investigated = new List<byte>();

        public Seer(PlayerControl player) : base(player)
        {
            Name = "Seer";
            StartText = () => "Reveal The Alliance Of Other Players";
            TaskText = () => "Reveal alliances of other players to find the Impostors";
            RoleAlignment = RoleAlignment.CrewInvest;
            Color = Colors.Seer;
            LastInvestigated = DateTime.UtcNow;
            RoleType = RoleEnum.Seer; 
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastInvestigated { get; set; }

        public float SeerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigated;
            var num = CustomGameOptions.SeerCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}