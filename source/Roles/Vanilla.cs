namespace TownOfSushi.Roles
{
    public class Impostor : Role
    {
        public Impostor(PlayerControl player) : base(player)
        {
            Name = "Impostor";
            StartText = () => "Kill All Crewmates";
            TaskText = () => "Kill all crewmates";
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleType = RoleEnum.Impostor;
            RoleAlignment = RoleAlignment.ImpSpecial;
            Color = Palette.ImpostorRed;
        }
    }
    public class Crewmate : Role
    {
        public Crewmate(PlayerControl player) : base(player)
        {
            Name = "Crewmate";
            StartText = () => "Find the Impostors";
            TaskText = () => "Find the Impostors";
            Faction = Faction.Crewmates;
            RoleAlignment = RoleAlignment.CrewSpecial;
            RoleType = RoleEnum.Crewmate;
            AddToRoleHistory(RoleType);
            Color = Colors.Crewmate;
        }
    }
}