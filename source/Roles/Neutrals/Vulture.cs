namespace TownOfSushi.Roles
{
    public class Vulture : Role
    {
        public int EatenBodies = 0;
        public int BodiesRemainingToWin()
        {
            return CustomGameOptions.VultureBodyCount - EatenBodies;
        }
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public Vulture(PlayerControl player) : base(player)
        {
            Name = "Vulture";
            StartText = () => "Eat dead bodies to win";
            TaskText = () => $"Eat dead bodies to win";
            
            Color = Colors.Vulture;
            RoleType = RoleEnum.Vulture;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            LastEaten = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.NeutralEvil;    
            EatNeed = CustomGameOptions.VultureBodyCount >= PlayerControl.AllPlayerControls._size / 2 ? PlayerControl.AllPlayerControls._size / 2 :
                CustomGameOptions.VultureBodyCount; //this line is from TOU reworked; thanks AD!
        }
        public DateTime LastEaten;
        public bool EatWin => EatNeed <= 0;
        public float EatTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastEaten;
            var num = CustomGameOptions.VultureCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
        public int EatNeed;
        public bool WonByEating { get; set; } = false;
        public DeadBody CurrentTarget;
        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            BodyArrows.Remove(arrow.Key);
        }
    }
}