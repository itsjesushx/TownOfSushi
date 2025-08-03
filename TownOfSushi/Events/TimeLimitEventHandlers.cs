﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using TownOfSushi.Options;
using TownOfSushi.Patches;

namespace TownOfSushi.Events;

public static class TimeLimitEventHandlers
{
    [RegisterEvent]
    public static void GameStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        if (TutorialManager.InstanceExists)
        {
            return; // Shouldn't run in Freeplay
        }

        if (!OptionGroupSingleton<GameTimerOptions>.Instance.GameTimerEnabled)
        {
            return;
        }

        // begin timer
        GameTimerPatch.BeginTimer();
    }
}