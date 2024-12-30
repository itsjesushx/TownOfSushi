namespace TownOfSushi.Roles.Modifiers
{
    public class DoubleShot : Modifier
    {
        public bool LifeUsed;
        public DoubleShot(PlayerControl player) : base(player)
        {
            Name = "Double Shot";
            TaskText = () => "You have an extra life when assassinating";
            Color = Colors.Impostor;
            ModifierType = ModifierEnum.DoubleShot;
            LifeUsed = false;
        }
    }
}