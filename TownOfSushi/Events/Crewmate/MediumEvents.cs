﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Modifiers;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class MediumEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        if (PlayerControl.LocalPlayer.Data.Role is MediumRole medium)
        {
            medium.MediatedPlayers.ForEach(mediated => mediated.Player?.RpcRemoveModifier(mediated.UniqueId));
        }
    }
}
