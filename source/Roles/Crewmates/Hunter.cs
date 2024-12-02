namespace TownOfSushi.Roles
{
    public class Hunter : Role
    {
        public Hunter(PlayerControl player) : base(player)
        {
            Name = "Hunter";
            StartText = () => "Stalk The <color=#FF0000FF>Impostor</color>";
            TaskText = () => "Stalk and kill impostors, but not crewmates";
            Color = Colors.Hunter;
            LastStalked = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Hunter;
            AddToRoleHistory(RoleType);
            UsesLeft = CustomGameOptions.HunterStalkUses;
        }

        private KillButton _stalkButton;
        public PlayerControl ClosestPlayer;
        public PlayerControl ClosestStalkPlayer;
        public PlayerControl StalkedPlayer;
        public PlayerControl LastVoted;
        public List<PlayerControl> CaughtPlayers = new List<PlayerControl>();
        public bool Enabled { get; set; }
        public DateTime LastStalked { get; set; }
        public float StalkDuration { get; set; }
        public DateTime LastKilled { get; set; }
        public int UsesLeft { get; set; }
        public TMPro.TextMeshPro UsesText { get; set; }
        public KillButton StalkButton
        {
            get => _stalkButton;
            set
            {
                _stalkButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public bool Stalking => StalkDuration > 0f;
        public bool StalkUsable => UsesLeft != 0;
        public float HunterKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.HunterKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float StalkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStalked;
            var num = CustomGameOptions.HunterStalkCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Stalk()
        {
            Enabled = true;
            StalkDuration -= Time.deltaTime;
        }

        public void StopStalking()
        {
            Enabled = false;
            LastStalked = DateTime.UtcNow;
            StalkedPlayer = null;
        }

        public void RpcCatchPlayer(PlayerControl stalked)
        {
            if (PlayerControl.LocalPlayer.PlayerId == Player.PlayerId && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Flash(Colors.Hunter, 1f);
            }
            CaughtPlayers.Add(stalked);
            StalkDuration = 0;
            StopStalking();
        }
    }
}