﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;

namespace TownOfSushi.Roles.Crewmate;

public static class SnitchEvents
{
    [RegisterEvent]
    public static void DeathEvent(PlayerDeathEvent @event)
    {
        if (SnitchRole.IsTargetOfSnitch(@event.Player))
        {
            CustomRoleUtils.GetActiveRolesOfType<SnitchRole>().ToList()
                .ForEach(snitch => snitch.RemoveArrowForPlayer(@event.Player.PlayerId));
        }
    }

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is SnitchRole snitch)
        {
            snitch.CheckTaskRequirements();
        }
    }
}