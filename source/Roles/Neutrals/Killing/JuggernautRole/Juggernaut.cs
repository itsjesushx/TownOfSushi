﻿namespace TownOfSushi.Roles
{
    public class Juggernaut : Role
    {
        public Juggernaut(PlayerControl owner) : base(owner)
        {
            Name = "Juggernaut";
            Color = Colors.Juggernaut;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Juggernaut;
            StartText = () => "Your Power Grows With Every Kill";
            TaskText = () => "With each kill your kill cooldown decreases";
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public int JuggKills { get; set; } = 0;
        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * JuggKills) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}