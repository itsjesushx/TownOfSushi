namespace TownOfSushi.Patches 
{
    [HarmonyPatch]
    class GetStringPatch 
    {
        [HarmonyPatch(typeof(TranslationController), nameof(TranslationController.GetString), new[] {
                typeof(StringNames),
                typeof(Il2CppReferenceArray<Il2CppSystem.Object>)
            })]
        public static bool Prefix(TranslationController __instance, StringNames id, ref string __result) {
            if ((int)id < 6000) {
                return true;
            }
            string ourString = "";

            // For now only do this in custom options.
            int idInt = (int)id - 6000;
            CustomOption.CustomOption opt = CustomOption.CustomOption.Options.FirstOrDefault(x => x.id == idInt);
            ourString = opt?.name;
            __result = ourString;
            return false;
        }
    }
}