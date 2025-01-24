namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipAssassinExileControllerPatch
    {
        public static void Postfix(AirshipExileController __instance) => AssassinExileControllerPatch.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPriority(Priority.First)]
    class AssassinExileControllerPatch
    {
        public static List<PlayerControl> AssassinatedPlayers = new List<PlayerControl>();
        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (var player in AssassinatedPlayers)
            {
                try
                {
                    if (!player.Data.Disconnected) player.Exiled();
                }
                catch { }
            }
            AssassinatedPlayers.Clear();
        }
    }
}