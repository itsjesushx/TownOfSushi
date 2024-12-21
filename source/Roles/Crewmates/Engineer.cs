namespace TownOfSushi.Roles.Crewmates
{
    public class Engineer : Role
    {
        public Engineer(PlayerControl player) : base(player)
        {
            Name = "Engineer";
            StartText = () => "Fix sabotages and vent around the map";
            TaskText = () => "Vent around and fix sabotages";
            
            Color = Colors.Engineer;
            RoleType = RoleEnum.Engineer;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
            MaxUses = CustomGameOptions.MaxFixes;
        }

        public int MaxUses;
        public TMPro.TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
    }
}