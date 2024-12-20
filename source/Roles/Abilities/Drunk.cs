namespace TownOfSushi.Roles.Abilities
{
    public class Drunk : Ability
    {
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "Inverrrrrted contrrrrols";
            Color = Colors.Drunk;
            AbilityType = AbilityEnum.Drunk;
        }
    }
}