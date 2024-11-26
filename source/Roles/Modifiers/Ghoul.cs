namespace TownOfSushi.Roles.Modifiers
{
    public class Ghoul : Modifier
    {
        public Ghoul(PlayerControl player) : base(player)
        {
            Name = "Ghoul";
            TaskText = () => "You are the karma of killers";
            Color = Colors.Impostor;
            ModifierType = ModifierEnum.Ghoul;
        }
    }
}