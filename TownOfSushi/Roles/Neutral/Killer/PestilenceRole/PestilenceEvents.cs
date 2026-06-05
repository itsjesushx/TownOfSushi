using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public static class PestilenceEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (MeetingHud.Instance || !Utils.TwoPlayersAlive())
        {
            return;
        }

        var pest = CustomRoleUtils.GetActiveRolesOfType<PestilenceRole>().FirstOrDefault();
        if (pest != null)
        {
            pest.Player.RpcRemoveModifier<InvulnerabilityModifier>();
            if (pest.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>You are now killable as a 1v1 is happening!</b>",
                    Color.white, spr: TownOfSushiAssets.Pestilence.LoadAsset());                
                notif1.AdjustNotification();
            }
        }
    }
}