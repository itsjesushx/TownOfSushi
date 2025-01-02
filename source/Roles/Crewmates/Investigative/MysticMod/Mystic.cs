namespace TownOfSushi.Roles.Crewmates
{
    public class Mystic : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public PlayerControl LastExaminedPlayer;
        public DateTime LastExamined { get; set; }
        public Mystic(PlayerControl player) : base(player)
        {
            Name = "Mystic";
            StartText = () => "Find out about the kills";
            TaskText = () => "Understand Kills & investigate about other players";
            Color = Colors.Mystic;
            RoleType = RoleEnum.Mystic;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
        }
        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.MysticExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}