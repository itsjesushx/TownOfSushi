﻿using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Roles;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class AurialEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button;
        var source = PlayerControl.LocalPlayer;

        if (!source.AmOwner || button == null || !button.CanClick())
        {
            return;
        }

        CustomRoleUtils.GetActiveRolesOfType<AurialRole>().Do(x => AurialRole.RpcSense(x.Player, source));
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;

        if (!source.AmOwner)
        {
            return;
        }

        CustomRoleUtils.GetActiveRolesOfType<AurialRole>().Do(x => AurialRole.RpcSense(x.Player, source));
    }
}