using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Options;
using UnityEngine;
using TownOfSushi.Modules;
using TownOfSushi.Modifiers.Game.Killer;

namespace TownOfSushi.Roles.Neutral;

public static class AmnesiacEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;
        var source = PlayerControl.LocalPlayer;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }
        if (source.HasModifier<RuthlessModifier>())
        {
            return;
        }

        CheckForAmnesiacVest(@event, target);
    }

    [RegisterEvent]
    public static void MiraButtonCancelledEventHandler(MiraButtonCancelledEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button is not IKillButton)
        {
            return;
        }

        if (target && !target!.HasModifier<AmnesiacVestModifier>())
        {
            return;
        }

        ResetButtonTimer(source, button);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (CheckForAmnesiacVest(@event, target))
        {
            ResetButtonTimer(source);
        }
    }

    private static bool CheckForAmnesiacVest(MiraCancelableEvent @event, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return false;
        }

        if (!target.HasModifier<AmnesiacVestModifier>())
        {
            return false;
        }

        @event.Cancel();

        return true;
    }

    private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null)
    {
        var reset = OptionGroupSingleton<GeneralOptions>.Instance.TempSaveCdReset;

        button?.SetTimer(reset);

        // Reset impostor kill cooldown if they attack a shielded player
        if (!source.AmOwner || !source.IsImpostor())
        {
            return;
        }

        if (source.HasModifier<RuthlessModifier>())
        {
            return;
        }

        source.SetKillTimer(reset);
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;

        if (!PlayerControl.LocalPlayer.IsRole<AmnesiacRole>()) return;

        var playerMenu = CustomPlayerMenu.Create();
        playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
        playerMenu.Begin(
            plr => (plr.HasDied() || UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
            FakePlayer.FakePlayers.FirstOrDefault(x => x?.body?.name == $"Fake {plr.gameObject.name}")?.body) && plr != PlayerControl.LocalPlayer,
            plr =>
            {
                playerMenu.ForceClose();

                if (plr != null)
                {
                    var targetId = plr.PlayerId;
                    var targetPlayer = MiscUtils.PlayerById(targetId);

                    if (targetPlayer == null) return;

                    AmnesiacRole.RpcRemember(PlayerControl.LocalPlayer, targetPlayer);
                }
            });
        foreach (var panel in playerMenu.potentialVictims)
        {
            panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
        }
    }
}