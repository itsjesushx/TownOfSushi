namespace TownOfSushi.Roles.Modifiers
{
    public class Mini : Modifier, IVisualAlteration
    {
        public Mini(PlayerControl player) : base(player)
        {
            Name = "Mini";
            TaskText = () => "You are tiny";
            Color = Colors.Mini;
            ModifierType = ModifierEnum.Mini;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SizeFactor = new Vector3(0.40f, 0.40f, 1f);
            return true;
        }
    }
}