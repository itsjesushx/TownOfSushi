using HarmonyLib;
using MiraAPI.LocalSettings;

namespace TownOfSushi.Patches
{
    // Class from: https://github.com/SuperNewRoles/SuperNewRoles
    [HarmonyPatch(typeof(LobbyBehaviour))]
    public static class LobbyBehaviourPatch
    {
        [HarmonyPatch(nameof(LobbyBehaviour.Update)), HarmonyPostfix]
        public static void Update_Postfix(LobbyBehaviour __instance)    
        {
            Func<ISoundPlayer, bool> uglyMusic = x => x.Name.Equals("MapTheme", StringComparison.Ordinal);
            ISoundPlayer LobbyMusic = SoundManager.Instance.soundPlayers.Find(uglyMusic);
            if (LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.DisableLobbyMusic.Value)
            {
                if (LobbyMusic == null) return;
                SoundManager.Instance.StopNamedSound("MapTheme");
            }
            else
            {
                if (LobbyMusic != null) return;
                SoundManager.Instance.CrossFadeSound("MapTheme", __instance.MapTheme, 0.5f);
            }
        }
    }
}