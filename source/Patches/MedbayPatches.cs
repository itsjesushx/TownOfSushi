namespace TownOfSushi
{
    internal class MedScan
	{

		[HarmonyPatch(typeof(MedScanMinigame))]
		private static class MedScanMinigamePatch
		{
			[HarmonyPatch(nameof(MedScanMinigame.Begin))]
			[HarmonyPostfix]
			private static void BeginPostfix(MedScanMinigame __instance)
			{
				// Update medical details for Giant modifier
				if (PlayerControl.LocalPlayer.Is(ModifierEnum.Giant))
				{
					__instance.completeString = __instance.completeString.Replace("3' 6\"", "5' 3\"").Replace("92lb", "184lb");
				}
				if (PlayerControl.LocalPlayer.Is(ModifierEnum.Mini))
				{
					__instance.completeString = __instance.completeString.Replace("3' 6\"", "2' 4\"").Replace("92lb", "45lb");
				}
			}
		}
	}
	[HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
}