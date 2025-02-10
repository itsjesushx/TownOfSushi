namespace TownOfSushi.Roles.Modifiers
{
    public class Celebrity : Modifier
    {
        public bool ShowFactions => CustomGameOptions.ShowCelebrityFaction;
        public Celebrity(PlayerControl player) : base(player)
        {
            Name = "Celebrity";
            TaskText = () => "Notify everyone when you die";
            Color = ColorManager.Celebrity;
            ModifierType = ModifierEnum.Celebrity;
        }
    }
}