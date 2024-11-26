
using TMPro;

namespace TownOfSushi.Roles
{
    public class Jailor : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Jailed;
        public bool CanJail;
        public GameObject ExecuteButton = new GameObject();
        public GameObject JailCell = new GameObject();
        public TMP_Text UsesText = new TMP_Text();
        public int Executes { get; set; }

        public Jailor(PlayerControl player) : base(player)
        {
            Name = "Jailor";
            StartText = () => "Jail and execute the <color=#FF0000FF>Impostors</color>";
            TaskText = () => "Execute and speak to the <color=#FF0000FF>Killers</color>";
            AlignmentName = "<color=#8BFDFDFF>Crew</color> (<color=#1D7CF2FF>Killing</color>)";
            Color = Colors.Jailor;
            LastJailed = DateTime.UtcNow;
            Alignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Jailor;
            AddToRoleHistory(RoleType);
            Executes = CustomGameOptions.MaxExecutes;
            CanJail = true;
        }
        public DateTime LastJailed { get; set; }
        public float JailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastJailed;
            var num = CustomGameOptions.JailCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}