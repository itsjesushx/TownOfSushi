namespace TownOfSushi.Roles.Modifiers
{
    public class Mini : Modifier
    {
        public Mini(PlayerControl player) : base(player)
        {
            var speedText = CustomGameOptions.MiniSpeed >= 1.50 ? " and fast!" : "!";
            Name = "Mini";
            TaskText = () => "You are tiny" + speedText;
            Color = Colors.Mini;
            ModifierType = ModifierEnum.Mini;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.MiniSpeed;
            appearance.SizeFactor = new Vector3(0.40f, 0.40f, 1f);
            return true;
        }
    }
}