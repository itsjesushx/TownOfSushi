using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Events.Neutral;

public static class JesterEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;
        if (OptionGroupSingleton<JesterOptions>.Instance.JestWin is JestWinOptions.EndsGame) return;
        var jest = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(plr => plr.Data.IsDead && !plr.Data.Disconnected && plr.GetRoleWhenAlive() is JesterRole jestRole && jestRole.Voted && !jestRole.SentWinMsg);
        if (jest != null)
        {
            var jestRole = jest.GetRoleWhenAlive() as JesterRole;
            if (jestRole == null) return;
            jestRole.SentWinMsg = true;
            
            if (jest.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>You have successfully won as the {TownOfSushiColors.Jester.ToTextColor()}Jester</color>, by getting voted out!</b>", Color.white, spr: TosRoleIcons.Jester.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                    notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>The {TownOfSushiColors.Jester.ToTextColor()}Jester</color>, {jest.Data.PlayerName}, has successfully won, as they were voted out!</b>", Color.white, spr: TosRoleIcons.Jester.LoadAsset());

                notif1.Text.SetOutlineThickness(0.35f);
                    notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            }

            if (OptionGroupSingleton<JesterOptions>.Instance.JestWin is not JestWinOptions.Haunts) return;
            if (!jest.AmOwner) return;

            var voters = jestRole.Voters.ToArray();
            Func<PlayerControl, bool> _playerMatch = plr => voters.Contains(plr.PlayerId) && !plr.HasDied() && !plr.HasModifier<InvulnerabilityModifier>() && plr != PlayerControl.LocalPlayer;

            var killMenu = CustomPlayerMenu.Create();
            killMenu.Begin(
                _playerMatch,
                plr =>
                {
                    killMenu.ForceClose();

                    if (plr != null)
                    {
                        PlayerControl.LocalPlayer.RpcCustomMurder(plr, teleportMurderer: false);
                    }
                });
            }
    }
}
