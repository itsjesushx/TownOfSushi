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

        public void Maul()
        {
            foreach (var player in GetClosestPlayers(Player.GetTruePosition(), CustomGameOptions.MaulRadius))
            {
                if (player.IsProtected() || Player == player || ClosestPlayer == player || player.IsShielded() || player == ShowRoundOneShield.FirstRoundShielded)
                    continue;

                if (!player.Is(RoleEnum.Pestilence))
                    RpcMurderPlayerNoJump(Player, player);

                if (player.IsOnAlert() || player.Is(RoleEnum.Pestilence))
                    RpcMurderPlayer(player, Player);

                if (player.IsInfected() || Player.IsInfected())
                {
                    foreach (var pb in GetRoles(RoleEnum.Plaguebearer))
                        ((Plaguebearer)pb).RpcSpreadInfection(player, Player);
                }
            }
        }
    }
}