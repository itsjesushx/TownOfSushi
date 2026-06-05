using HarmonyLib;
using TownOfSushi.Options;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(MapBehaviour))]
	static class MapBehaviourPatch 
	{

		[HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
		static void Postfix(MapBehaviour __instance) 
		{
			if (__instance.infectedOverlay.gameObject.active && PlayerControl.LocalPlayer.Data.IsDead && OptionGroupSingleton<GeneralOptions>.Instance.DeadImpsBlockSabotage) 
			{
				__instance.Close();
			}
        }
    }
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdatePatch
    {
        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        [HarmonyPostfix]
        public static void Postfix(HudManager __instance)
        {
            if (MeetingHud.Instance || Utils.TwoPlayersAlive() && OptionGroupSingleton<GeneralOptions>.Instance.LimitAbilities)
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
            if ((PlayerControl.LocalPlayer.Data.IsDead && OptionGroupSingleton<GeneralOptions>.Instance.DeadImpsBlockSabotage)
             || (Utils.TwoPlayersAlive() && OptionGroupSingleton<GeneralOptions>.Instance.LimitAbilities))
            {
                __instance.ShowNormalMap();
                return false;
            }

            return true;
        }
    }
}