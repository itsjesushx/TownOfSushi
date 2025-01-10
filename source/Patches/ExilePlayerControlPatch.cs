namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileController2
    {
        public static NetworkedPlayerInfo lastExiled;
        public static void Prefix(ExileController __instance, [HarmonyArgument(0)]ref NetworkedPlayerInfo exiled)
        {
            lastExiled = exiled;

            var exiled2 = exiled.Object;
            var role = GetPlayerRole(exiled2);

            role.DeathReason = DeathReasonEnum.Ejected;
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerPatch
    {
        public static ExileController lastExiled;
        public static void Prefix(ExileController __instance)
        {
            lastExiled = __instance;
        }
    }

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class ResetCustomTimersPatch
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
                ResetCustomTimers();
        }
    }
}