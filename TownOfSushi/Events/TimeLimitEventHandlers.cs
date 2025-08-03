﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Patches;

namespace TownOfSushi.Events;

public static class TimeLimitEventHandlers
{
    [RegisterEvent]
    public static void GameStartEventHandler(RoundStartEvent @event)
    {
        if (!@event.TriggeredByIntro) return; // Only run when round starts.

        // begin timer
        GameTimerPatch.BeginTimer();
    }
}
