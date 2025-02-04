namespace TownOfSushi.Roles
{
    public abstract class Ability
    {
        public static readonly Dictionary<byte, Ability> AbilityDictionary = new Dictionary<byte, Ability>();
        public Func<string> TaskText;
        protected Ability(PlayerControl player)
        {
            Player = player;
            AbilityDictionary.Add(player.PlayerId, this);
        }
        public static IEnumerable<Ability> AllAbilities => AbilityDictionary.Values.ToList();
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
        protected internal AbilityEnum AbilityType { get; set; }
        public string ColorString => "<color=#" + Color.ToHtmlStringRGBA() + ">";

        private bool Equals(Ability other)
        {
            return Equals(Player, other.Player) && AbilityType == other.AbilityType;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(Ability)) return false;
            return Equals((Ability)obj);
        }
        public static T GenAbility<T>(Type type, PlayerControl player)
        {
            var ability = (T)Activator.CreateInstance(type, new object[] { player });
            StartRPC(CustomRPC.SetAbility, player.PlayerId, (string)type.FullName);
            return ability;
        }

        public static T GenAbility<T>(Type type, List<PlayerControl> players)
        {
            var player = players[Random.RandomRangeInt(0, players.Count)];
            var ability = GenAbility<T>(type, player);
            players.Remove(player);
            return ability;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Player, (int)AbilityType);
        }

        public static bool operator ==(Ability a, Ability b)
        {
            if (a is null && b is null) return true;
            if (a is null || b is null) return false;
            return a.AbilityType == b.AbilityType && a.Player.PlayerId == b.Player.PlayerId;
        }

        public static bool operator !=(Ability a, Ability b)
        {
            return !(a == b);
        }

        public static Ability GetAbility(PlayerControl player)
        {
            return (from entry in AbilityDictionary where entry.Key == player.PlayerId select entry.Value)
                .FirstOrDefault();
        }

        public static T GetAbility<T>(PlayerControl player) where T : Ability
        {
            return GetAbility(player) as T;
        }

        public static Ability GetAbility(PlayerVoteArea area)
        {
            var player = PlayerControl.AllPlayerControls.ToArray()
                .FirstOrDefault(x => x.PlayerId == area.TargetPlayerId);
            return player == null ? null : GetAbility(player);
        }

        public static IEnumerable<Ability> GetAbilities(AbilityEnum abilitytype)
        {
            return AllAbilities.Where(x => x.AbilityType == abilitytype);
        }

        public void ReDoTaskText()
        {
            var ability = GetAbility(Player);
            var task = new GameObject(ability.Name + "Task").AddComponent<ImportantTextTask>();
            
            task.transform.SetParent(Player.transform, false);
            task.Text = $"{ability.ColorString}Ability: {ability.Name}\n{ability.TaskText()}</color>";
            Player.myTasks.Insert(0, task);
            Player.myTasks.ToArray()[5].Cast<ImportantTextTask>().Text =
            $"{ability.ColorString}Ability: {ability.Name}\n{ability.TaskText()}</color>";
        }

        public static Ability GetAbilityValue(AbilityEnum abilityEnum)
        {
            foreach (var ability in AllAbilities)
            {
                if (ability.AbilityType == abilityEnum)
                    return ability;
            }

            return null;
        }

        public static T GetAbilityValue<T>(AbilityEnum abilityEnum) where T : Ability => GetAbilityValue(abilityEnum) as T;
    }
}