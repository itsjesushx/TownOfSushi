﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Impostor;

public static class ScavengerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || !source.IsRole<ScavengerRole>()) return;

        var scavenger = source.GetRole<ScavengerRole>();
        scavenger?.OnPlayerKilled(@event.Target);
    }
}
