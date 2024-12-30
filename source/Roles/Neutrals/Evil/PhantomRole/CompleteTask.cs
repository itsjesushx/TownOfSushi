namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Phantom)) return;
            var role = GetRole<Phantom>(__instance);

            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == 0 && !role.Caught)
            {
                role.CompletedTasks = true;
            }
        }
    }
}