namespace TownOfSushi.Roles.Crewmates
{
    public class Mayor : Role
    {
        public Mayor(PlayerControl player) : base(player)
        {
            Name = "Mayor";
            StartText = () => "Reveal yourself to save everyone";
            TaskText = () => "Reveal yourself when the time is right";
            Color = Colors.Mayor;
            RoleType = RoleEnum.Mayor;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewPower;
            Revealed = false;
        }
        public bool Revealed { get; set; }
        public GameObject RevealButton = new GameObject();
        internal override bool Criteria()
        {
            return Revealed && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }
    }
}