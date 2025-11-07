using System.Collections;
using HarmonyLib;
using MiraAPI.LocalSettings;
using Reactor.Utilities;
using TownOfSushi.Modules;
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
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, msg);
        }

        if (!SentOnce && LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowWelcomeMessageToggle.Value)
        {
            var name = "<color=#8BFDFD>Automated Message</color>";
            var msg =
                $"Welcome to Town of Sushi v{TownOfSushiPlugin.Version}!\nUse the wiki (the globe icon) to get more info on roles or modifiers, where you can use the searchbar. Otherwise use /help in the chat to get a list of commands.\nYou can also disable this message through your options menu.";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, name, msg, true);
            time = 5f;
        }
        else if (!LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowWelcomeMessageToggle.Value)
        {
            time = 2.48f;
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