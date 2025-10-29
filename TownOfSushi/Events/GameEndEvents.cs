using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameEnd;
using Reactor.Utilities.Extensions;
using TownOfSushi.GameOver;
using TownOfSushi.Modules;
using TownOfSushi.Patches;

namespace TownOfSushi.Events;

public static class EndGameEvents
{
    public static int winType;

    [RegisterEvent(-100)]
    public static void OnGameEnd(GameEndEvent @event)
    {
        winType = 0;
        var reason = EndGameResult.CachedGameOverReason;
        var neutralWinner = CustomRoleUtils.GetActiveRolesOfTeam(ModdedRoleTeams.Custom)
            .Any(x => x is ITownOfSushiRole role && role.WinConditionMet());

        if (neutralWinner)
        {
            return;
        }

        if (reason is GameOverReason.CrewmatesByVote or GameOverReason.CrewmatesByTask
            or GameOverReason.ImpostorDisconnect)
        {
            winType = 1;
            GameHistory.WinningFaction = $"<color=#{Palette.CrewmateBlue.ToHtmlStringRGBA()}>Crewmates</color>";
        }
        else if (reason is GameOverReason.ImpostorsByKill or GameOverReason.ImpostorsBySabotage
                 or GameOverReason.ImpostorsByVote or GameOverReason.CrewmateDisconnect)
        {
            winType = 2;
            GameHistory.WinningFaction = $"<color=#{Palette.ImpostorRed.ToHtmlStringRGBA()}>Impostors</color>";
        }

        if (reason == CustomGameOver.GameOverReason<DrawGameOver>())
        {
            winType = 0;
        }
    }

    [RegisterEvent]
    public static void GameEndEventHandler(GameEndEvent @event)
    {
        EndGamePatches.BuildEndGameSummary(@event.EndGameManager);
    }
}