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
            role.LastMorph = DateTime.UtcNow;
            if (Player == PlayerControl.LocalPlayer)
            {
                Flash(Colors.Hitman);
                SoundManager.Instance.PlaySound(ShipStatus.Instance.SabotageSound, false, 1f, null);
                role.RegenTask();
            }
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class AgentTaskUpdate
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Agent)) return;
            var role = GetRole<Agent>(__instance);
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0 )
            {
                role.ChangeRole();
            }
        }
    }
}