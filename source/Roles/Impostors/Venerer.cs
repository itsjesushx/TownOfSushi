namespace TownOfSushi.Roles
{
    public class Venerer : Role
    {
        public KillButton _abilityButton;
        public bool Enabled;
        public DateTime LastCamouflaged;
        public float TimeRemaining;
        public float KillsAtStartAbility;

        public Venerer(PlayerControl player) : base(player)
        {
            Name = "Venerer";
            StartText = () => "With Each Kill Your Ability Becomes Stronger";
            TaskText = () => "Kill players to unlock ability perks";
            Color = Colors.Impostor;
            LastCamouflaged = DateTime.UtcNow;
            RoleType = RoleEnum.Venerer;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpDeception;
        }

        public bool IsCamouflaged => TimeRemaining > 0f;
        public KillButton AbilityButton
        {
            get => _abilityButton;
            set
            {
                _abilityButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float AbilityTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCamouflaged;
            var num = CustomGameOptions.AbilityCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ability()
        {
            
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
            GroupCamouflage();
        }


        public void StopAbility()
        {
            Enabled = false;
            LastCamouflaged = DateTime.UtcNow;
            UnCamouflage();
        }
    }
}