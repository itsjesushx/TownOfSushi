namespace TownOfSushi.Roles.Abilities
{
    public class Sleuth : Ability
    {
        public List<byte> Reported = new List<byte>();
        public Sleuth(PlayerControl player) : base(player)
        {
            Name = "Sleuth";
            TaskText = () => "Know the roles of bodies you report";
            Color = Colors.Sleuth;
            AbilityType = AbilityEnum.Sleuth;
        }
    }
}