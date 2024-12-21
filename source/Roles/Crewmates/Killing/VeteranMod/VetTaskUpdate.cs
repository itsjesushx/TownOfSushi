namespace TownOfSushi.Roles.Crewmates.Killing.VeteranMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class TasksPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Veteran)) return;
            var role = GetRole<Veteran>(__instance);
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);

            if (tasksLeft == CustomGameOptions.VeteranTaskNeed)
            {
                role.MaxUses++;
            }
        }
    }
}