namespace TownOfSushi.Roles
{
    public class Amnesiac : Role
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new Dictionary<byte, ArrowBehaviour>();
        public bool SpawnedAs = true;

        public Amnesiac(PlayerControl player) : base(player)
        {
            Name = "Amnesiac";
            StartText = () => "Remember A Role Of A Deceased Player";
            TaskText = () => SpawnedAs ? "Find a dead body to remember a role" : "Your target died. Now remember a new role!";
            
            Color = Colors.Amnesiac;
            RoleType = RoleEnum.Amnesiac;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralBenign;
        }

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