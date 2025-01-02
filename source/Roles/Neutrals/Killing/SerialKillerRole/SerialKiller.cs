namespace TownOfSushi.Roles
{
    public class SerialKiller : Role
    {
        private KillButton _rampageButton;
        public bool Enabled;
        public PlayerControl ClosestPlayer;
        public DateTime LastStabbed;
        public DateTime LastKilled;
        public float TimeRemaining;
        public SerialKiller(PlayerControl player) : base(player)
        {
            Name = "Serial Killer";
            StartText = () => "Stab to kill everyone";
            TaskText = () => "Stab to kill everyone";
            Color = Colors.SerialKiller;
            LastStabbed = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.SerialKiller;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public KillButton StabButton
        {
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        
        public bool Stabbed => TimeRemaining > 0f;
        public float StabTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStabbed;
            var num = CustomGameOptions.StabCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Stab()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void Unrampage()
        {
            Enabled = false;
            LastStabbed = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.StabKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}