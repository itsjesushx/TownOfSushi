namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(IGameOptionsExtensions), nameof(IGameOptionsExtensions.GetAdjustedNumImpostors))]
    public class GetAdjustedImposters
    {
        public static bool Prefix(IGameOptions __instance, ref int __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return true;
            if (CustomGameOptions.GameMode == GameMode.AllAny && CustomGameOptions.RandomNumberImps)
            {
                var players = GameData.Instance.PlayerCount;

                var impostors = 1;
                var random = Random.RandomRangeInt(0, 100);
                if (players <= 6) impostors = 1;
                else if (players <= 7)
                {
                    if (random < 20) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 8)
                {
                    if (random < 40) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 9)
                {
                    if (random < 50) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 10)
                {
                    if (random < 60) impostors = 2;
                    else impostors = 1;
                }
                else if (players <= 11)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 70) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 12)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 80) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 13)
                {
                    if (random < 60) impostors = 2;
                    else if (random < 90) impostors = 3;
                    else impostors = 1;
                }
                else if (players <= 14)
                {
                    if (random < 50) impostors = 3;
                    else impostors = 2;
                }
                else
                {
                    if (random < 60) impostors = 3;
                    else if (random < 90) impostors = 2;
                    else impostors = 4;
                }
                __result = impostors;
                return false;
            }
            return true;
        }
    }
    
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Initialize))]
    public class EnableMapImps
    {
        private static void Prefix(ref GameOptionsMenu __instance)
        {
            __instance.HideForOnline = new Il2CppReferenceArray<Transform>(0);
        }
    }
}
