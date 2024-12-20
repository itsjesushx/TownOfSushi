namespace TownOfSushi.Roles.Abilities
{
    public class Radar : Ability
    {
        public List<ArrowBehaviour> RadarArrow = new List<ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public Radar(PlayerControl player) : base(player)
        {
            Name = "Radar";
            TaskText = () => "Be on high alert";
            Color = Colors.Radar;
            AbilityType = AbilityEnum.Radar;
        }
    }
}