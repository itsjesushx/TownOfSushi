using TMPro;
using TownOfSushi.Roles.Crewmates.Investigative.TrapperMod;

namespace TownOfSushi.Roles.Crewmates
{
    public class Trapper : Role
    {
        public static Material trapMaterial = TownOfSushi.bundledAssets.Get<Material>("trap");
        public List<Trap> traps = new List<Trap>();
        public DateTime LastTrapped { get; set; }
        public int UsesLeft;
        public TextMeshPro UsesText;
        public List<RoleEnum> trappedPlayers;
        public bool ButtonUsable => UsesLeft != 0;
        public Trapper(PlayerControl player) : base(player)
        {
            Name = "Trapper";
            StartText = () => "Catch Killers In The Act";
            TaskText = () => "Place traps around the map";
            Color = Colors.Trapper;
            RoleType = RoleEnum.Trapper;
            LastTrapped = DateTime.UtcNow;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewInvest;
            trappedPlayers = new List<RoleEnum>();
            UsesLeft = CustomGameOptions.MaxTraps;
        }

        public float TrapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTrapped;
            var num = CustomGameOptions.TrapCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
