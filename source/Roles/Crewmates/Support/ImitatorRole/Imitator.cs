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
        public List<RoleEnum> ImitatableRoles = new List<RoleEnum>
        {
            RoleEnum.Investigator, RoleEnum.Mystic, RoleEnum.Seer, RoleEnum.Tracker, RoleEnum.Vigilante, 
            RoleEnum.Veteran, RoleEnum.Engineer, RoleEnum.Medium, RoleEnum.Transporter, RoleEnum.Trapper, 
            RoleEnum.Medic, RoleEnum.Oracle, RoleEnum.Hunter
        };
        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            StartText = () => "Use the true-hearted dead to benefit the crew";
            TaskText = () => "Use dead roles to benefit the crew";
            Color = Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
        }
    }
}