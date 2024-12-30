namespace TownOfSushi.Roles.Abilities
{
    public class Torch : Ability
    {
        public Torch(PlayerControl player) : base(player)
        {
            Name = "Torch";
            TaskText = () => "You can see in the dark";
            Color = Colors.Torch;
            AbilityType = AbilityEnum.Torch;
        }
    }
}