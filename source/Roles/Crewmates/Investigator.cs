using TownOfSushi.Roles.Crewmates.Investigative.InvestigatorMod;

namespace TownOfSushi.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        private KillButton _examineButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastExamined { get; set; }
        public DeadBody CurrentTarget;
        public bool ExamineMode = false;
        public PlayerControl DetectedKiller;
        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            StartText = () => "Find all Impostors by examining footprints";
            TaskText = () => "Watch steps and examine players";
            LastExamined = DateTime.UtcNow;
            Color = Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
        }
        public KillButton ExamineButton
        {
            get => _examineButton;
            set
            {
                _examineButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float ExamineTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastExamined;
            var num = CustomGameOptions.ExamineCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}