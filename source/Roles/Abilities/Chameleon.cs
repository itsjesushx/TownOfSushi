namespace TownOfSushi.Roles.Abilities
{
    public class Chameleon : Ability
    {
        public bool Moving = false;
        public DateTime LastMoved { get; set; }
        public float Opacity = 1;
        public Chameleon(PlayerControl player) : base(player)
        {
            Name = "Chameleon";
            TaskText = () => "You're hard to see when you're not moving";
            Color = Colors.Chameleon;
            AbilityType = AbilityEnum.Chameleon;
            LastMoved = DateTime.UtcNow;
        }
    }
}