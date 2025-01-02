namespace TownOfSushi.Roles
{
    public class Morphling : Role, IVisualAlteration
    {
        public KillButton _morphButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMorphed;
        public PlayerControl MorphedPlayer;
        public PlayerControl SampledPlayer;
        public float TimeRemaining;

        public Morphling(PlayerControl player) : base(player)
        {
            Name = "Morphling";
            StartText = () => "Transform Into Crewmates";
            TaskText = () => "Morph into crewmates to be disguised";
            Color = Colors.Impostor;
            LastMorphed = DateTime.UtcNow;
            RoleType = RoleEnum.Morphling;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpDeception;
        }

        public KillButton MorphButton
        {
            get => _morphButton;
            set
            {
                _morphButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public bool Morphed => TimeRemaining > 0f;

        public void MorphlingMorph()
        {            
            TimeRemaining -= Time.deltaTime;
            Morph(Player, MorphedPlayer);
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void MorphlingUnmorph()
        {
            MorphedPlayer = null;
            Unmorph(Player);
            LastMorphed = DateTime.UtcNow;
        }

        public float MorphTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMorphed;
            var num = CustomGameOptions.MorphlingCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (Morphed)
            {
                appearance = MorphedPlayer.GetDefaultAppearance();
                var modifier = GetModifier(MorphedPlayer);
                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);
                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }
    }
}
