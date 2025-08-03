﻿using HarmonyLib;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using TownOfSushi.Roles;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class GameManagerPatches
{
    public static int winType;
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.DidImpostorsWin))]
    [HarmonyPatch(typeof(GameManager), nameof(GameManager.DidHumansWin))]
    [HarmonyPrefix]
    public static bool DidHumansOrImpostorsWinPatch(GameManager __instance, GameOverReason reason, ref bool __result)
    {
        winType = 0;
        var neutralWinner = CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom).Any(x => x is ITownOfSushiRole role && role.WinConditionMet());

        if (neutralWinner)
        {
            __result = false;
            return false;
        }

        if (reason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask or GameOverReason.ImpostorDisconnect)
        {
            winType = 1;
            GameHistory.WinningFaction = $"<color=#{Palette.CrewmateBlue.ToHtmlStringRGBA()}>Crewmates</color>";
        }
        else if (reason is GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage or GameOverReason.ImpostorsByVote or GameOverReason.CrewmateDisconnect)
        {
            winType = 2;
            GameHistory.WinningFaction = $"<color=#{Palette.ImpostorRed.ToHtmlStringRGBA()}>Impostors</color>";
        }

        return true;
    }

    // [HarmonyPatch(typeof(GameManager), nameof(GameManager.RpcEndGame))]
    // [HarmonyPostfix]
    // public static void RpcEndGamePatch(GameManager __instance, [HarmonyArgument(0)] GameOverReason endReason)
    // {
    //    Logger<TownOfSushiPlugin>.Message($"RpcEndGamePatch - GameOverReason: {endReason}");
    // }
}
