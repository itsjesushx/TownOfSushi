using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using TownOfSushi.Patches;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public static class PhantomEvents
{
    [RegisterEvent]
    public static void CompleteTaskEventHandler(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is not PhantomTOSRole phantom)
        {
            return;
        }

        phantom.CheckTaskRequirements();

        if (phantom.CompletedAllTasks &&
            OptionGroupSingleton<PhantomOptions>.Instance.PhantomWin is not PhantomWinOptions.EndsGame)
        {
            phantom.Clicked();
            if (phantom.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Phantom, $"<b>You have successfully won as the Phantom, as you finished your tasks postmortem!</b>"),
                    Color.white, spr: TOSRoleIcons.Phantom.LoadAsset());

                
                notif1.AdjustNotification();
                HudManagerPatches.ZoomButton.SetActive(true);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Phantom, $"<b>The Phantom, {phantom.Player.Data.PlayerName}, has successfully won, as they completed their tasks postmortem!</b>"),
                    Color.white, spr: TOSRoleIcons.Phantom.LoadAsset());

                
                notif1.AdjustNotification();
            }
        }
    }
}