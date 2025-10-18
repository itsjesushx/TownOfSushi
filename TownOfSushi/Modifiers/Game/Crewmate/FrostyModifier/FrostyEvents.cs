using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;
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
                MiscUtils.ColorString(TownOfSushiColors.Frosty, $"<b>{@event.Target.Data.PlayerName} was Frosty, causing you to be slower for {Math.Round(OptionGroupSingleton<FrostyOptions>.Instance.ChillDuration, 2)} seconds.</b>"),
                Color.white, spr: TOSModifierIcons.Frosty.LoadAsset());

            
            notif1.AdjustNotification();
        }

        @event.Source.AddModifier<FrozenModifier>();
    }
}