namespace TownOfSushi.Roles
{
    public class Werewolf : Role
    {
        private KillButton _maulButton;
        public PlayerControl ClosestPlayer;
        public DateTime LastMauled;
        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            StartText = () => "Murder and eliminate everyone";
            TaskText = () => "Kill everyone";
            Color = Colors.Werewolf;
            LastMauled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.Neutral;
            RoleAlignment = RoleAlignment.NeutralKilling;
        }
        public KillButton MaulButton
        {
            get => _maulButton;
            set
            {
                _maulButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float MaulTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMauled;
            var num = CustomGameOptions.MaulCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Maul(PlayerControl player2)
        {
            var closestPlayers = GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius, false);

            foreach (var player in closestPlayers)
            {
                if (player == Player)
                   continue;

                if (player.IsProtected())
                    continue;
                    
                if (player != Player && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                    RpcMurderPlayer(player2, player);
                
                if (player.IsOnAlert() || !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                    RpcMurderPlayer(player, player2);
                
                if (player.IsInfected() || Player.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(player, Player);
                }
            }
        }
    }
}