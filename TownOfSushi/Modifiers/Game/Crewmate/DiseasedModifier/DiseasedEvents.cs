﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public static class DiseasedEvents
{
    [RegisterEvent(10)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (!target.HasModifier<DiseasedModifier>() || target == source || MeetingHud.Instance)
        {
            return;
        }

        var cdMultiplier = OptionGroupSingleton<DiseasedOptions>.Instance.CooldownMultiplier;
        if (source.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Diseased.ToTextColor()}{@event.Target.Data.PlayerName} was Diseased, causing your kill cooldown to multiply by {Math.Round(cdMultiplier, 2)}.</color></b>",
                Color.white, spr: TOSModifierIcons.Diseased.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }

        source.SetKillTimer(source.GetKillCooldown() * cdMultiplier);
        var buttons = CustomButtonManager.Buttons.Where(x => x.Enabled(source.Data.Role)).OfType<IDiseaseableButton>();

        foreach (var button in buttons)
        {
            button.SetDiseasedTimer(cdMultiplier);
        }
    }
}