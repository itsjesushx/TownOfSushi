using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using TownOfSushi.Patches;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public static class SpectreEvents
{
    [RegisterEvent]
    public static void CompleteTaskEventHandler(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is not SpectreRole Spectre)
        {
            return;
        }

        Spectre.CheckTaskRequirements();

        if (Spectre.CompletedAllTasks &&
            OptionGroupSingleton<SpectreOptions>.Instance.SpectreWin is not SpectreWinOptions.EndsGame)
        {
            Spectre.Clicked();
            if (Spectre.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Spectre, $"<b>You have successfully won as the Spectre, as you finished your tasks postmortem!</b>"),
                    Color.white, spr: TOSRoleIcons.Spectre.LoadAsset());

                
                notif1.AdjustNotification();
                HudManagerPatches.ZoomButton.SetActive(true);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Spectre, $"<b>The Spectre, {Spectre.Player.Data.PlayerName}, has successfully won, as they completed their tasks postmortem!</b>"),
                    Color.white, spr: TOSRoleIcons.Spectre.LoadAsset());

                
                notif1.AdjustNotification();
            }
        }
    }
}