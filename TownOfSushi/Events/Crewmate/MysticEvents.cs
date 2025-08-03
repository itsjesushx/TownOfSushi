﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class MysticEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (MeetingHud.Instance) return;

        var victim = @event.Target;

        if (PlayerControl.LocalPlayer.Data.Role is MysticRole)
        {
            victim?.AddModifier<MysticDeathNotifierModifier>(PlayerControl.LocalPlayer);
        }
    }
}
