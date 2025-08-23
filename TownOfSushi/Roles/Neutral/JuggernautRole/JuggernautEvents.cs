﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Neutral;

public static class JuggernautEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || source.Data.Role is not JuggernautRole juggernaut || MeetingHud.Instance)
        {
            return;
        }

        juggernaut.KillCount++;
        CustomButtonSingleton<JuggernautKillButton>.Instance.ResetCooldownAndOrEffect();
    }
}