namespace TownOfSushi.Roles.Crewmates
{
    public class Guardian : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Target;
        public Faction RevealedFaction;
        public bool ProtectedPlayer;
        public DateTime LastProtect { get; set; }
        public Guardian(PlayerControl player) : base(player)
        {
            Name = "Guardian";
            StartText = () => "Protect a player from being ejected";
            TaskText = () => "Protect a player";
            Color = Colors.Guardian;
            LastProtect = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewSupport;
            RoleType = RoleEnum.Guardian;
            AddToRoleHistory(RoleType);
        }
        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtect;
            var num = CustomGameOptions.VoteProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}