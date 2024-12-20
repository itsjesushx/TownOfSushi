using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public bool LastKiller = false;
        public int DousedAlive => DousedPlayers.Count(x => PlayerById(x) != null && PlayerById(x).Data != null && !PlayerById(x).Data.IsDead && !PlayerById(x).Data.Disconnected);
        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            StartText = () => "Douse Players And Ignite The Light";
            TaskText = () => "Douse players and ignite to kill all douses";
            Color = Colors.Arsonist;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = PlayerById(playerId);
                if (!player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    RpcMultiMurderPlayer(Player, player);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            DousedPlayers.Clear();
        }
    }
}