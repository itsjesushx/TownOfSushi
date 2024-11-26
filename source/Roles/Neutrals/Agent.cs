namespace TownOfSushi.Roles
{
    public class Agent : Role
    {
        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = () => "Finish tasks to gain powers";
            TaskText = () => "Finish your duties to murder the crew";
            Color = Colors.Agent;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }

        public void ChangeRole()
        {
            var oldRole = GetPlayerRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Hitman(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            if (Player == PlayerControl.LocalPlayer)
            {
                Flash(Colors.Hitman);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                role.RegenTask();
            }
        }
    }
}