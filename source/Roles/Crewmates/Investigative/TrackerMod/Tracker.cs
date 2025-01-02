

using TMPro;

namespace TownOfSushi.Roles.Crewmates
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked { get; set; }
        public int MaxUses;
        public TextMeshPro UsesText;
        public bool ButtonUsable => MaxUses != 0;
        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            StartText = () => "Track Everyone's Movement";
            TaskText = () => "Track suspicious players";
            Color = Colors.Tracker;
            LastTracked = DateTime.UtcNow;
            RoleType = RoleEnum.Tracker;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            MaxUses = CustomGameOptions.MaxTracks;
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TrackerArrows.Remove(arrow.Key);
        }
    }
}