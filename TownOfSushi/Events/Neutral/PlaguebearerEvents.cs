﻿using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Events.Neutral;

public static class PlaguebearerEvents
{
    [RegisterEvent]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        if (!@event.Target)
        {
            return;
        }

        PlaguebearerRole.CheckInfected(@event.Target!.Object, @event.Reporter);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (MeetingHud.Instance)
        {
            return;
        }

        PlaguebearerRole.CheckInfected(@event.Source, @event.Target);
    }

    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (@event.Button is PlaguebearerInfectButton)
        {
            return;
        }

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }

        PlaguebearerRole.RpcCheckInfected(source, target);
    }

    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        var pest = CustomRoleUtils.GetActiveRolesOfType<PestilenceRole>().FirstOrDefault(x => !x.Announced);
        if (pest != null)
        {
            pest.Announced = true;
            if (pest.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>The crew know of the {TownOfSushiColors.Pestilence.ToTextColor()}Pestilence</color>.</b>",
                    Color.white, spr: TOSRoleIcons.Pestilence.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>The {TownOfSushiColors.Plaguebearer.ToTextColor()}plague</color> has consumed the crew. {TownOfSushiColors.Pestilence.ToTextColor()}Pestilence</color>, Horseman of the Apocalypse, has emerged!</b>",
                    Color.white, spr: TOSRoleIcons.Pestilence.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
        }
    }
}