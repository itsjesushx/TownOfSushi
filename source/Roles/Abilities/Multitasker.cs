namespace TownOfSushi.Roles.Abilities
{
    public class Multitasker : Ability
    {
        public Multitasker(PlayerControl player) : base(player)
        {
            Name = "Multitasker";
            TaskText = () => "Your task windows are transparent";
            Color = Colors.Multitasker;
            AbilityType = AbilityEnum.Multitasker;
        }
    }
}