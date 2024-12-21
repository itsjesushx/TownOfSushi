namespace TownOfSushi.Roles.Crewmates
{
    public class Oracle : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Confessor;
        public float Accuracy;
        public bool FirstMeetingDead;
        public RoleAlignment RevealedAlignment;
        public Faction RevealedFaction;
        public DateTime LastConfessed { get; set; }

        public Oracle(PlayerControl player) : base(player)
        {
            Name = "Oracle";
            StartText = () => "Get other payer's to confess their sins";
            TaskText = () => "Get another player to confess on your passing";
            Color = Colors.Oracle;
            LastConfessed = DateTime.UtcNow;
            Accuracy = CustomGameOptions.RevealAccuracy;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            FirstMeetingDead = true;
            FirstMeetingDead = false;
            RoleType = RoleEnum.Oracle;
        }
        public float ConfessTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConfessed;
            var num = CustomGameOptions.ConfessCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}