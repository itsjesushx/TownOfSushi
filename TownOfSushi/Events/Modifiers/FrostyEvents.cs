﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;
using TownOfSushi.Options.Modifiers.Crewmate;
using UnityEngine;

namespace TownOfSushi.Events.Modifiers;

public static class FrostyEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!@event.Target.HasModifier<FrostyModifier>() || @event.Target == @event.Source ||
            MeetingHud.Instance)
        {
            return;
        }

        if (@event.Source.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Frosty.ToTextColor()}{@event.Target.Data.PlayerName} was Frosty, causing you to be slower for {Math.Round(OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration, 2)} seconds.</color></b>",
                Color.white, spr: TOSModifierIcons.Frosty.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }

        @event.Source.AddModifier<FrozenModifier>();
    }
}