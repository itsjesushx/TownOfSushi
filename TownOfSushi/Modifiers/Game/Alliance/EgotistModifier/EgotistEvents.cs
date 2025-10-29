using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public static class EgotistEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        var ego = ModifierUtils.GetActiveModifiers<EgotistModifier>().FirstOrDefault(x => !x.Player.HasDied());
        if (ego != null && Helpers.GetAlivePlayers().Where(x =>
                    x.IsCrewmate() && !(x.TryGetModifier<AllianceGameModifier>(out var ally) && !ally.GetsPunished))
                .ToList().Count == 0)
        {
            if (ego.Player.AmOwner)
            {
                PlayerControl.LocalPlayer.RpcPlayerExile();
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Egotist, $"<b>You have successfully won as the Egotist") + ", as no more crewmates remain!</b>",
                    Color.white, spr: TOSModifierIcons.Egotist.LoadAsset());

                
                notif1.AdjustNotification();
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Egotist, $"<b>The Egotist") + $", {ego.Player.Data.PlayerName}, has successfully won, as no more crewmates remain!</b>",
                    Color.white, spr: TOSModifierIcons.Egotist.LoadAsset());

                
                notif1.AdjustNotification();
            }
        }
    }
}