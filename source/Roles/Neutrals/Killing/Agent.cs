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
            Color = ColorManager.Agent;
            RoleType = RoleEnum.Agent;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralKilling;
        }

        public void ChangeRole()
        {
            var role = GetPlayerRole(Player);
            var killsList = (role.Kills, role.CorrectKills,  role.CorrectDeputyShot, role.CorrectShot, role.IncorrectShots, role.CorrectVigilanteShot, role.CorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role2 = new Hitman(Player);
            role2.Kills = killsList.Kills;
            role2.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            role2.CorrectKills = killsList.CorrectKills;
            role2.IncorrectShots = killsList.IncorrectShots;
            role2.CorrectShot = killsList.CorrectShot;
            role2.CorrectDeputyShot = killsList.CorrectDeputyShot;
            role2.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role2.LastMorph = DateTime.UtcNow;
            if (Player == PlayerControl.LocalPlayer)
            {
                Flash(ColorManager.Hitman);
                Sound().PlaySound(Ship().SabotageSound, false, 1f, null);
                role.ReDoTaskText();
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