namespace TownOfSushi.Roles.Crewmates
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public Swapper(PlayerControl player) : base(player)
        {
            Name = "Swapper";
            StartText = () => "Swap the votes of two people";
            TaskText = () => "Swap two people's votes to save the Crew!";
            Color = Colors.Swapper;
            RoleType = RoleEnum.Swapper;
            RoleAlignment = RoleAlignment.CrewSupport;
            AddToRoleHistory(RoleType);
        }
    }
}