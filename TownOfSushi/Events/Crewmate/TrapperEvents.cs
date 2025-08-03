﻿using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Roles;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class TrapperEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is TrapperRole && OptionGroupSingleton<TrapperOptions>.Instance.TaskUses && !OptionGroupSingleton<TrapperOptions>.Instance.TrapsRemoveOnNewRound)
        {
            var button = CustomButtonSingleton<TrapperTrapButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }
    [RegisterEvent]
    public static void StartMeetingEventHandler(StartMeetingEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<TrapperRole>().Do(x => x.Report());
    }
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (OptionGroupSingleton<TrapperOptions>.Instance.TrapsRemoveOnNewRound) CustomRoleUtils.GetActiveRolesOfType<TrapperRole>().Do(x => x.Clear());
    }
}
