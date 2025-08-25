using HarmonyLib;
using TownOfSushi.Options;
using TaskLength = NormalPlayerTask.TaskLength;

namespace TownOfSushi.Patches.Options;

[HarmonyPatch]
public static class TaskAssignmentPatch
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.AddTasksFromList))]
    [HarmonyPrefix]
    public static void Prefix(ShipStatus __instance, ref int count, ref Il2CppSystem.Collections.Generic.List<NormalPlayerTask> unusedTasks)
    {
        var type = unusedTasks[0].Length;

        if (type is TaskLength.Short)
        {
            count += OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedShortTasks();
            count = Math.Clamp(count, 0, __instance.ShortTasks.Count);
        }
        else if (type is TaskLength.Long)
        {
            count += OptionGroupSingleton<TownOfSushiMapOptions>.Instance.GetMapBasedLongTasks();
            count = Math.Clamp(count, 0, __instance.LongTasks.Count);
        }
        else if (type is TaskLength.Common)
        {
            count = Math.Clamp(count, 0, __instance.CommonTasks.Count);
        }
    }

    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
    [HarmonyPrefix]
    public static void Prefix()
    {
        var options = GameOptionsManager.Instance.currentNormalGameOptions;
        options.NumCommonTasks = Math.Clamp(options.NumCommonTasks, 0, 4);
        options.NumShortTasks = Math.Clamp(options.NumShortTasks, 0, 8);
        options.NumLongTasks = Math.Clamp(options.NumLongTasks, 0, 4);
    }
}