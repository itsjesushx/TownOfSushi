namespace TownOfSushi.Roles.Abilities
{
    public class Flash : Ability, IVisualAlteration
    {

        public Flash(PlayerControl player) : base(player)
        {
            Name = "Flash";
            TaskText = () => "You are faster than others";
            Color = Colors.Flash;
            AbilityType = AbilityEnum.Flash;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.FlashSpeed;
            return true;
        }
    }
}