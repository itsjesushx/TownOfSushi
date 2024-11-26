namespace TownOfSushi.Roles
{
    public class Executioner : Role
    {
        public PlayerControl target;
        public Executioner(PlayerControl player) : base(player)
        {
            Name = "Executioner";
            StartText = () =>
                target == null ? "You don't have a target for some reason... weird..." : $"Vote {target.name} Out";
            TaskText = () =>
                target == null
                    ? "You don't have a target for some reason... weird..."
                    : $"Vote {target.name} out!";
            
            
            Color = Colors.Executioner;
            RoleType = RoleEnum.Executioner;
            
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
            Scale = 1.4f;
        }
        public bool TargetVotedOut;

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            TargetVotedOut = true;
        }
    }
}