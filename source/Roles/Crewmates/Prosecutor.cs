namespace TownOfSushi.Roles.Crewmates
{
    public class Prosecutor : Role
    {
        public Prosecutor(PlayerControl player) : base(player)
        {
            Name = "Prosecutor";
            StartText = () => "Exile One Person Of Your Choosing";
            TaskText = () => "Choose to exile anyone you want";
            AlignmentName = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Power</color>)";
            Color = Colors.Prosecutor;
            RoleType = RoleEnum.Prosecutor;
            HasProsecuted = false;
            StartProsecute = false;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewPower;
            Prosecuted = false;
            ProsecuteThisMeeting = false;
        }
        public bool ProsecuteThisMeeting { get; set; }
        public bool Prosecuted { get; set; }
        public bool StartProsecute { get; set; }
        public PlayerVoteArea Prosecute { get; set; }
        public bool HasProsecuted { get; set; }
    }
}
