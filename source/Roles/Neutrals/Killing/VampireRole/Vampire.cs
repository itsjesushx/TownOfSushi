namespace TownOfSushi.Roles
{
    public class Vampire : Role
    {
        public Vampire(PlayerControl player) : base(player)
        {
            Name = "Vampire";
            StartText = () => "Convert Crewmates and kill the rest";
            TaskText = () => "Conver players and kill everyone to win";
            
            Color = Colors.Vampire;
            LastBit = DateTime.UtcNow;
            RoleType = RoleEnum.Vampire;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastBit { get; set; }
        public float BiteTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBit;
            var num = CustomGameOptions.BiteCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}