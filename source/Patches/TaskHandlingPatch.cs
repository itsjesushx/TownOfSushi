namespace TownOfSushi.Patches
{
    internal static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts
        {
            private static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (var i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    var playerInfo = __instance.AllPlayers.ToArray()[i];
                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object &&
                        (VanillaOptions().currentNormalGameOptions.GhostsDoTasks || !playerInfo.IsDead) && !playerInfo.IsImpostor() &&
                        !(
                            playerInfo._object.Is(Faction.Neutral) 
                        ))
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
                        }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse
        {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] NetworkedPlayerInfo playerInfo, ref float __result, ref bool canUse, ref bool couldUse)
            {
                var playerControl = playerInfo.Object;

                var flag = 
                playerControl.Is(Faction.Neutral)
                && !playerControl.Is(RoleEnum.Amnesiac)
                && !playerControl.Is(RoleEnum.GuardianAngel)
                && !playerControl.Is(RoleEnum.Agent);

                if (flag && !__instance.AllowImpostor)
                {
                    __result = float.MaxValue;
                    canUse = false;
                    couldUse = false;
                    return false;
                }
                return true;
            }
        }
    }
    
    [HarmonyPatch(typeof(ShipStatus))]
    public class ShipStatusPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
        public static void Postfix(LogicGameFlowNormal __instance, ref bool __result)
        {
            __result = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Begin))]
        public static bool Prefix(ShipStatus __instance)
        {
                var commonTask = __instance.CommonTasks.Count;
                var normalTask = __instance.ShortTasks.Count;
                var longTask = __instance.LongTasks.Count;
                if (VanillaOptions().currentNormalGameOptions.NumCommonTasks > commonTask) VanillaOptions().currentNormalGameOptions.NumCommonTasks = commonTask;
                if (VanillaOptions().currentNormalGameOptions.NumShortTasks > normalTask) VanillaOptions().currentNormalGameOptions.NumShortTasks = normalTask;
                if (VanillaOptions().currentNormalGameOptions.NumLongTasks > longTask) VanillaOptions().currentNormalGameOptions.NumLongTasks = longTask;
            
            return true;
        }
    }
}