using HarmonyLib;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.GetPurchase))]
    public static class GetPurchasePatch
    {
        public static bool Prefix(ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3400:Methods should not return constants", Justification = "Required by Harmony patching.")]
    [HarmonyPatch(typeof(PlayerPurchasesData), nameof(PlayerPurchasesData.SetPurchased))]
    public static class SetPurchasedPatch
    {
        public static bool Prefix() => false;
    }
}