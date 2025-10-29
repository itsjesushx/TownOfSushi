using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;
using Reactor.Utilities;
using TownOfSushi.Options;

using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public static class MonarchEvents
{
    [RegisterEvent]
    public static void HandleVoteEvent(HandleVoteEvent @event)
    {
        if (!@event.VoteData.Owner.HasModifier<MonarchKnightedModifier>()) return;

        @event.VoteData.SetRemainingVotes(0);

        for (var i = 0; i < 2; i++)
        {
            @event.VoteData.VoteForPlayer(@event.TargetId);
        }

        @event.Cancel();
    }

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.AmOwner && @event.Player.Data.Role is MonarchRole && OptionGroupSingleton<MonarchOptions>.Instance.TaskUses)
        {
            var button = CustomButtonSingleton<KnightButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    // Can't interact with Monarch if Knighted at all.
    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (source.HasModifier<MonarchKnightedModifier>() && target.Data.Role is MonarchRole)
        {
            @event.Cancel();
            ResetButtonTimer(source);
            if (source.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Monarch, PlaySound: true));
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Monarch,
                $"<b>You cannot kill {target.Data.PlayerName} because they are the Monarch who knighted you.</color></b>"),
                Color.white, spr: TOSCrewAssets.KnightSprite.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (target.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Monarch, PlaySound: true));
                var notif2 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Monarch,
                $"<b>{source.Data.PlayerName} tried to kill you.</color></b>"),
                Color.white, spr: TOSCrewAssets.KnightSprite.LoadAsset());
                
                notif2.AdjustNotification();
            }
        }
    }

    private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null)
    {
        var reset = OptionGroupSingleton<GeneralOptions>.Instance.TempSaveCdReset;

        button?.SetTimer(reset);

        if (!source.AmOwner || !source.IsImpostor())
        {
            return;
        }

        source.SetKillTimer(reset);
    }

    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
            return;

        if (source.HasModifier<MonarchKnightedModifier>() && target.Data.Role is MonarchRole)
        {
            @event.Cancel();
            ResetButtonTimer(source, button);
            if (source.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Monarch,
                $"<b>You cannot interact with {target.Data.PlayerName} (Monarch) while Knighted.</color></b>"),
                Color.white, spr: TOSCrewAssets.KnightSprite.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (target.AmOwner)
            {
                var notif2 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Monarch,
                $"<b>{source.Data.PlayerName} tried to interact with you.</color></b>"),
                Color.white, spr: TOSCrewAssets.KnightSprite.LoadAsset());
                
                notif2.AdjustNotification();
            }
        }
    }
}