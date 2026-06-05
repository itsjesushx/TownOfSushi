using HarmonyLib;
using MiraAPI.LocalSettings;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class LobbyBehaviourPatches
{
    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    [HarmonyPostfix]
    public static void LobbyStartPatch(LobbyBehaviour __instance)
    {
        foreach (var role in GameHistory.AllRoles)
        {
            if (!role || role is not ITOSRole tosRole)
            {
                continue;
            }

            tosRole.LobbyStart();
        }

        GameHistory.ClearAll();
        ScreenFlash.Clear();
        MeetingMenu.ClearAll();
    }
}

// Class from: https://github.com/SuperNewRoles/SuperNewRoles
[HarmonyPatch(typeof(LobbyBehaviour))]
public static class LobbyBehaviourPatch
{
    [HarmonyPatch(nameof(LobbyBehaviour.Update)), HarmonyPostfix]
    public static void Update_Postfix(LobbyBehaviour __instance)    
    {
        Func<ISoundPlayer, bool> mapThemeMusic = x => x.Name.Equals("MapTheme", StringComparison.Ordinal);
        ISoundPlayer LobbyMusic = SoundManager.Instance.soundPlayers.Find(mapThemeMusic);
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