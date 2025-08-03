﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Events.Vanilla.Usables;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class TransporterEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is TransporterRole &&
            OptionGroupSingleton<TransporterOptions>.Instance.TaskUses)
        {
            var button = CustomButtonSingleton<TransporterTransportButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    [RegisterEvent]
    public static void PlayerCanUseEventHandler(PlayerCanUseEvent @event)
    {
        if (OptionGroupSingleton<TransporterOptions>.Instance.CanUseVitals)
        {
            return;
        }

        if (PlayerControl.LocalPlayer == null ||
            PlayerControl.LocalPlayer.Data == null ||
            PlayerControl.LocalPlayer.Data.Role is not TransporterRole)
        {
            return;
        }

        var console = @event.Usable.TryCast<SystemConsole>();

        if (console == null)
            // Not a SystemConsole, return
        {
            return;
        }

        if (console.MinigamePrefab.TryCast<VitalsMinigame>())
        {
            @event.Cancel();
        }
    }
}