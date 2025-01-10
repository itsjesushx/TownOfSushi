namespace TownOfSushi.Roles
{
    public abstract class Modifier
    {
        public static readonly Dictionary<byte, Modifier> ModifierDictionary = new Dictionary<byte, Modifier>();
        public Func<string> TaskText;
        protected Modifier(PlayerControl player)
        {
            Player = player;
            ModifierDictionary.Add(player.PlayerId, this);
        }

        public static T GenModifier<T>(Type type, PlayerControl player)
        {
            var modifier = (T)Activator.CreateInstance(type, new object[] { player });
            StartRPC(CustomRPC.SetModifier, player.PlayerId, (string)type.FullName);
            return modifier;
        }
        public static T GenModifier<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];

            var modifier = GenModifier<T>(type, player);
            players.Remove(player);
            return modifier;
        }

        public static IEnumerable<Modifier> AllModifiers => ModifierDictionary.Values.ToList();
        protected internal string Name { get; set; }
        public string PlayerName { get; set; }
        private PlayerControl _player { get; set; }
        public PlayerControl Player
        {
            get => _player;
            set
            {
                if (_player != null) _player.nameText().color = Color.white;

                _player = value;
                PlayerName = value.Data.PlayerName;
            }
        }
        protected internal Color Color { get; set; }
        protected internal ModifierEnum ModifierType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Modifier other)
        {
            return Equals(Player, other.Player) && ModifierType == other.ModifierType;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Modifier)) return false;
            return Equals((Modifier) obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int) ModifierType);
        }

        public static bool operator ==(Modifier a, Modifier b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.ModifierType == b.ModifierType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Modifier a, Modifier b)
        {
            return !(a == b);
        }

        public static Modifier GetModifier(PlayerControl player)
        {
            return (from entry in ModifierDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static IEnumerable<Modifier> GetModifiers(ModifierEnum modifiertype)
        {
            return AllModifiers.Where(x => x.ModifierType == modifiertype);
        }

        public static T GetModifier<T>(PlayerControl player) where T : Modifier
        {
            return GetModifier(player) as T;
        }

        public static Modifier GetModifier(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetModifier(player);
        }
    }
}