using TownOfSushi.Roles.Impostors.Power.BomberRole;
using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles
{
    public class Bomber : Role
    {
        public KillButton _plantButton;
        public float TimeRemaining;
        public bool Enabled = false;
        public bool Detonated = true;
        public Vector3 DetonatePoint;
        public Bomb Bomb = new Bomb();
        public static Material bombMaterial = TownOfSushi.bundledAssets.Get<Material>("bomb");
        public DateTime StartingCooldown { get; set; }
        public Bomber(PlayerControl player) : base(player)
        {
            Name = "Bomber";
            StartText = () => "Plant Bombs To Kill Multiple Crewmates At Once";
            TaskText = () => "Plant bombs to kill crewmates";
            Color = Palette.ImpostorRed;
            StartingCooldown = DateTime.UtcNow;
            RoleType = RoleEnum.Bomber;
            Faction = Faction.Impostors;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
        }
        public KillButton PlantButton
        {
            get => _plantButton;
            set
            {
                _plantButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public bool Detonating => TimeRemaining > 0f;
        public void DetonateTimer()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (MeetingHud.Instance) Detonated = true;
            if (TimeRemaining <= 0 && !Detonated)
            {
                var bomber = GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.Bomb.ClearBomb();
                DetonateKillStart();
            }
        }
        public void DetonateKillStart()
        {
            Detonated = true;
            var playersToDie = GetClosestPlayers(DetonatePoint, CustomGameOptions.DetonateRadius);
            playersToDie = Shuffle(playersToDie);
            while (playersToDie.Count > CustomGameOptions.MaxKillsInDetonation) playersToDie.Remove(playersToDie[playersToDie.Count - 1]);
            foreach (var player in playersToDie)
            {
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
        }
        public static Il2CppSystem.Collections.Generic.List<PlayerControl> Shuffle(Il2CppSystem.Collections.Generic.List<PlayerControl> playersToDie)
        {
            var count = playersToDie.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i)
            {
                var r = Random.Range(i, count);
                var tmp = playersToDie[i];
                playersToDie[i] = playersToDie[r];
                playersToDie[r] = tmp;
            }
            return playersToDie;
        }
    }
}