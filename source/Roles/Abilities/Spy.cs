namespace TownOfSushi.Roles.Abilities
{
    public class Spy : Ability
    {
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            TaskText = () => "Gain extra information from all the devices";
            Color = Colors.Spy;
            AbilityType = AbilityEnum.Spy;
        }
    }
}