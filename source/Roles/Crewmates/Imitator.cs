namespace TownOfSushi.Roles.Crewmates
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public PlayerControl ImitatePlayer = null;
        public PlayerControl LastExaminedPlayer = null;
        public List<RoleEnum> trappedPlayers = null;
        public PlayerControl confessingPlayer = null;
        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            StartText = () => "Use The True-Hearted Dead To Benefit The Crew";
            TaskText = () => "Use dead roles to benefit the crew";
            Color = Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Support</color>)";
        }
    }
}