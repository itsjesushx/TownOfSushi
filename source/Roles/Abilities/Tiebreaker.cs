namespace TownOfSushi.Roles.Abilities
{
    public class Tiebreaker : Ability
    {
        public Tiebreaker(PlayerControl player) : base(player)
        {
            Name = "Tiebreaker";
            TaskText = () => "Your vote breaks ties";
            Color = Colors.Tiebreaker;
            AbilityType = AbilityEnum.Tiebreaker;
        }
    }
}