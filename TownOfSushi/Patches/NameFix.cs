namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(MapBehaviour))]
	static class MapInfectedOverlayPatch
	{

		[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
		static void Postfix(MapBehaviour __instance) 
		{
			if (__instance.infectedOverlay.gameObject.active && PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadImpsBlockSabotage) 
			{
				__instance.Close();
			}
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class SabotageHudManagerPatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            if (MeetingHud.Instance || Utils.TwoPlayersAlive() && CustomGameOptions.LimitAbilities)
            {
                __instance.SabotageButton.Hide();
            }
            
            else
            {
                if (PlayerControl.LocalPlayer.IsImpostor()) __instance.SabotageButton.Show();
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    public static class ShowSabotageMapPatch
    {
        public static bool Prefix(MapBehaviour __instance)
        {
            if ((PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadImpsBlockSabotage)
            || Grenadier.Active || (Utils.TwoPlayersAlive() && CustomGameOptions.LimitAbilities))
            {
                __instance.ShowNormalMap();
                return false;
            }

            return true;
        }
    }
}