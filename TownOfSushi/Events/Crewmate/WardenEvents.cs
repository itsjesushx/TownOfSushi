﻿using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Crewmate;

public static class WardenEvents
{
    [RegisterEvent(priority: -1)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        // Logger<TownOfSushiPlugin>.Error("WardenEvents KillButtonClickHandler");
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick()) return;

        CheckForWardenFortify(@event, source, target);
    }

    [RegisterEvent(priority: -1)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        CheckForWardenFortify(@event, source, target);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var victim = @event.Target;

        foreach (var warden in CustomRoleUtils.GetActiveRolesOfType<WardenRole>())
        {
            if (victim == warden.Fortified)
                warden.Clear();
        }
    }

    private static void CheckForWardenFortify(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (!target.HasModifier<WardenFortifiedModifier>() || source == target || MeetingHud.Instance || (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield)) return;
        @event.Cancel();

        // Find the warden which fortified the target
        var warden = target.GetModifier<WardenFortifiedModifier>()?.Warden.GetRole<WardenRole>();

        if (warden != null && source.AmOwner)
        {
            WardenRole.RpcWardenNotify(warden.Player, source, target);
        }
    }
}
