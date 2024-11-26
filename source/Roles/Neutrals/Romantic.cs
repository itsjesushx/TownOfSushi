namespace TownOfSushi.Roles
{
    public class Romantic : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Beloved;
        public DateTime LastPick;
        public bool SpawnedAs = true;
        public bool AlreadyPicked = false;
        public Dictionary<byte, ArrowBehaviour> RomanticArrows = new Dictionary<byte, ArrowBehaviour>();
        public Romantic(PlayerControl player) : base(player)
        {
            Name = "Romantic";
            StartText = () => "Pick a beloved to win with them";
            TaskText = () => SpawnedAs ? "Protect and assist your beloved" : "Your beloved died. Pick a new one!";
            Color = Colors.Romantic;
            RoleType = RoleEnum.Romantic;
            Faction = Faction.Neutral;
            LastPick = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
        }

        public float PickTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastPick;
            var num = CustomGameOptions.PickStartTimer * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}