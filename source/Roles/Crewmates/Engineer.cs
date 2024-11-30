namespace TownOfSushi.Roles.Crewmates
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = () => "Fix sabotages and vent around the map";
            TaskText = () => "Vent around and fix sabotages";
            AlignmentName = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Support</color>)";
            Color = Colors.Engineer;
            RoleType = RoleEnum.Engineer;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
            UsesLeft = CustomGameOptions.MaxFixes;
        }

        public int UsesLeft;
        public TMPro.TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
    }
}