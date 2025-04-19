namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(MapBehaviour))]
	static class MapBehaviourPatch 
	{
        [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
		static void Postfix(MapBehaviour __instance) 
		{
            // close settings when map is opened
            CustomOption.HudManagerUpdate.CloseSettings();
        }
    }
}