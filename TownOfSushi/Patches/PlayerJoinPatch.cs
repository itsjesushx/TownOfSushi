using System.Collections;
using HarmonyLib;
using MiraAPI.LocalSettings;
using Reactor.Utilities;

using UnityEngine;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
public static class PlayerJoinPatch
{
    public static bool SentOnce { get; private set; }
    public static HudManager HUD => HudManager.Instance;
    public static void Postfix()
    {
        Coroutines.Start(CoSendJoinMsg());
    }

    private static IEnumerator CoSendJoinMsg()
    {
        while (!AmongUsClient.Instance)
        {
            yield return null;
        }

        Logger<TownOfSushiPlugin>.Info("Client Initialized?");

        while (!PlayerControl.LocalPlayer)
        {
            yield return null;
        }

        var player = PlayerControl.LocalPlayer;

        while (!player)
        {
            yield return null;
        }

        if (!player.AmOwner)
        {
            yield break;
        }

        Logger<TownOfSushiPlugin>.Info("Sending Message to Local Player...");
        TOSRoleManagerPatches.ReplaceRoleManager = false;

        var time = 0f;
        if (GameHistory.EndGameSummary != string.Empty && LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowSummaryMessageToggle.Value)
        {
            var factionText = string.Empty;
            var msg = string.Empty;
            if (GameHistory.WinningFaction != string.Empty)
            {
                factionText = $"<size=80%>Winning Team: {GameHistory.WinningFaction}</size>\n";
            }

            var title =
                $"<color=#8BFDFD>Automated Message (Toggleable In Options)</color>\n<size=62%>{factionText}{GameHistory.EndGameSummary}</size>";
            Utils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);
        }

        if (time == 0)
        {
            yield break;
        }

        yield return new WaitForSeconds(time);
        Logger<TownOfSushiPlugin>.Info("Offset Wiki Button (if needed)");
        SentOnce = true;
    }
}