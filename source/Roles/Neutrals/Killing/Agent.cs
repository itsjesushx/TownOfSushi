namespace TownOfSushi.Roles
{
    public class Agent : Role
    {
        public Agent(PlayerControl player) : base(player)
        {
            Name = "Agent";
            StartText = () => "Finish tasks to gain powers";
            TaskText = () => "Finish your duties to murder the crew";
            RoleInfo = "The Agent is a Neutral role with its own win condition. The Agent has tasks like the Crewmates but their tasks do not count for a task win. After the Agent finishes its tasks, They automatically become a Hitman";
            LoreText = "A covert operative with a dark mission, you blend in with the crew while secretly advancing your own agenda. As the Agent, your goal is to complete tasks to unlock deadly powers that allow you to eliminate your fellow crewmates. The more tasks you finish, the closer you get to becoming a true threat. Use your growing strength wisely, for the crew will soon realize that you're more dangerous than you appear.";
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