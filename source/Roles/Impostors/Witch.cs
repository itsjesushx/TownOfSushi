namespace TownOfSushi.Roles
{
    public class Witch : Role
    {
        public KillButton _spellButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl Spelled;
        public DateTime LastSpelled{ get; set; }
        public Witch(PlayerControl player) : base(player)
        {
            Name = "Witch";
            StartText = () => "Cast a spell upon your foes";
            TaskText = () => "Cast a spell upon your foes";            
            Color = Colors.Impostor;
            LastSpelled = DateTime.UtcNow;
            RoleType = RoleEnum.Witch;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
        }

        public KillButton SpellButton
        {
            get => _spellButton;
            set
            {
                _spellButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float SpellTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastSpelled;
            var num = CustomGameOptions.SpellCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        private static Sprite spelledOverlaySprite;
        public static Sprite getSpelledOverlaySprite()
        {
            if (spelledOverlaySprite) return spelledOverlaySprite;
            spelledOverlaySprite = LoadSpriteFromResources("TownOfSushi.Resources.SpellButtonMeeting.png", 115f);
            return spelledOverlaySprite;
        }
    }
}