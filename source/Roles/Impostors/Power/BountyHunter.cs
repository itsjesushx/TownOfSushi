/* I will continue with this later
namespace TownOfSushi.Roles
{
    public class BountyHunter : Role
    {
        public PlayerControl Target;
        public DateTime LastPoisoned;
        public BountyHunter(PlayerControl player) : base(player)
        {
            Name = "Bounty Hunter";
            StartText = () => $"Hunt your bounties down";
            TaskText = () => "Hunt the crewmates";
            RoleInfo = $"";
            LoreText = "";
            Color = Palette.ImpostorRed;
            LastPoisoned = DateTime.UtcNow;
            RoleType = RoleEnum.BountyHunter;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.ImpPower;
            Faction = Faction.Impostors;
            Target = null;
        }
    }
}*/