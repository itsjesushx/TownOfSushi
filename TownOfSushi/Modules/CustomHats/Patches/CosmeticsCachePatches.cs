﻿
using TownOfSushi;

namespace TownOfSushi.Modules.CustomHats.Patches;

[HarmonyPatch(typeof(CosmeticsCache))]
internal static class CosmeticsCachePatches
{
    [HarmonyPatch(nameof(CosmeticsCache.GetHat))]
    [HarmonyPrefix]
    private static bool GetHatPrefix(string id, ref HatViewData __result)
    {
        TownOfSushiPlugin.Logger.LogMessage($"trying to load hat {id} from cosmetics cache");
        return !CustomHatManager.ViewDataCache.TryGetValue(id, out __result);
    }
}