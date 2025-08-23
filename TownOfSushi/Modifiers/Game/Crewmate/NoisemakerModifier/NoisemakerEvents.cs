﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public static class NoisemakerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.TryGetModifier<NoisemakerModifier>(out var noise) &&
            !@event.Source.IsRole<SoulCollectorRole>() && !MeetingHud.Instance)
        {
            noise.NotifyOfDeath(@event.Target);
        }
    }
}