﻿using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Modifiers.Game.Universal;

public static class ShyEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        ModifierUtils.GetActiveModifiers<ShyModifier>().Do(x => x.OnRoundStart());
    }
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<ShyModifier>())
        {
            ShyModifier.SetVisibility(@event.Target, 1f);
        }
    }
}