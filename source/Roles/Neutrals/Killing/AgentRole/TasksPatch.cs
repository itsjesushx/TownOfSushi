namespace TownOfSushi.Roles.Neutral.Killing.AgentRole
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class TasksPatch
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