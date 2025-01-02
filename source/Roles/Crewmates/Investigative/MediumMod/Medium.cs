

namespace TownOfSushi.Roles.Crewmates
{
    public class Medium : Role
    {
        public DateTime LastMediated { get; set; }
        public Dictionary<byte, ArrowBehaviour> MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        public static Sprite Arrow => TownOfSushi.Arrow;
        public Medium(PlayerControl player) : base(player)
        {
            Name = "Medium";
            StartText = () => "Watch The Spooky Ghosts";
            TaskText = () => "Follow ghosts to get clues from them";
            Color = Colors.Medium;
            LastMediated = DateTime.UtcNow;
            RoleType = RoleEnum.Medium;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            MediatedPlayers = new Dictionary<byte, ArrowBehaviour>();
        }

        internal override bool RoleCriteria()
        {
            return (MediatedPlayers.ContainsKey(PlayerControl.LocalPlayer.PlayerId) && CustomGameOptions.ShowMediumToDead) || base.RoleCriteria();
        }
        public float MediateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMediated;
            var num = CustomGameOptions.MediateCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public void AddMediatePlayer(byte playerId)
        {
            var gameObj = new GameObject();
            var arrow = gameObj.AddComponent<ArrowBehaviour>();
            if (Player.PlayerId == PlayerControl.LocalPlayer.PlayerId || CustomGameOptions.ShowMediumToDead)
            {
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Arrow;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerById(playerId).transform.position;
            }
            MediatedPlayers.Add(playerId, arrow);
            Flash(Color, 2.5f);
        }
    }
}