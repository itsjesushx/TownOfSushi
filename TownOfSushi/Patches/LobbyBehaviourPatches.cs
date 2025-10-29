using HarmonyLib;
using TownOfSushi.Modules;

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
            if (!role || role is not ITownOfSushiRole tosRole)
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