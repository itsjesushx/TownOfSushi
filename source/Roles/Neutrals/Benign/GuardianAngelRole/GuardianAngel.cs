using TMPro;

namespace TownOfSushi.Roles
{
    public class GuardianAngel : Role
    {
        public bool Enabled;
        public DateTime LastProtected;
        public float TimeRemaining;
        public int MaxUses;
        public TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
        public PlayerControl target;
        public GuardianAngel(PlayerControl player) : base(player)
        {
            Name = "Guardian Angel";
            StartText = () =>
                target == null ? "You don't have a target for some reason" : $"Protect {target.name} with your life!";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason"
                    : $"Protect {target.name}!";
            
            Color = Colors.GuardianAngel;
            LastProtected = DateTime.UtcNow;
            RoleType = RoleEnum.GuardianAngel;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
            MaxUses = CustomGameOptions.MaxProtects;
        }
        public bool Protecting => TimeRemaining > 0f;
        public float ProtectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastProtected;
            var num = CustomGameOptions.ProtectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void Protect()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
        }
        public void UnProtect()
        {
            var ga = GetRole<GuardianAngel>(Player);
            if (!ga.target.IsShielded())
            {
                ga.target.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                ga.target.myRend().material.SetFloat("_Outline", 0f);
            }
            Enabled = false;
            LastProtected = DateTime.UtcNow;
        }
    }
}