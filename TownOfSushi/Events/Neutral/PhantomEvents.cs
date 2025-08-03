using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Utilities;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Events.Neutral;

public static class PhantomEvents
{
    [RegisterEvent]
    public static void CompleteTaskEventHandler(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is not ModdedPhantomRole phantom) return;

        phantom.CheckTaskRequirements();

        if (phantom.CompletedAllTasks && OptionGroupSingleton<PhantomOptions>.Instance.PhantomWin is not PhantomWinOptions.EndsGame)
        {
            phantom.Clicked();
            if (phantom.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>You have successfully won as the {TownOfSushiColors.Phantom.ToTextColor()}Phantom</color>, as you finished your tasks postmortem!</b>", Color.white, spr: TosRoleIcons.Phantom.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
                Patches.HudManagerPatches.ZoomButton.SetActive(true);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>The {TownOfSushiColors.Phantom.ToTextColor()}Phantom</color>, {phantom.Player.Data.PlayerName}, has successfully won, as they completed their tasks postmortem!</b>", Color.white, spr: TosRoleIcons.Phantom.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
        }
    }
}
