﻿using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Crewmate;

public static class ProsecutorEvents
{
    [RegisterEvent]
    public static void VoteEvent(HandleVoteEvent @event)
    {
        if (@event.VoteData.Owner.Data.Role is not ProsecutorRole { HasProsecuted: true } prosecutorRole) return;
        if (prosecutorRole.ProsecutionsCompleted >=
            OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions) return;

        @event.VoteData.SetRemainingVotes(0);

        for (var i = 0; i < 5; i++)
        {
            @event.VoteData.VoteForPlayer(@event.TargetId);
        }

        foreach (var plr in PlayerControl.AllPlayerControls.ToArray().Where(player => player != @event.VoteData.Owner))
        {
            plr.GetVoteData().Votes.Clear();
            plr.GetVoteData().VotesRemaining = 0;
        }

        @event.Cancel();
    }

    [RegisterEvent]
    public static void WrapUpEvent(EjectionEvent @event)
    {
        var player = @event.ExileController.initData.networkedPlayer?.Object;
        if (player == null) return;

        foreach (var pros in CustomRoleUtils.GetActiveRolesOfType<ProsecutorRole>())
        {
            if (pros.HasProsecuted && player.IsCrewmate() && !(pros.Player.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished) && !(player.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished))
            {
                if (OptionGroupSingleton<ProsecutorOptions>.Instance.ExileOnCrewmate)
                {
                    pros.Player.Exiled();
                }
                else
                {
                    pros.ProsecutionsCompleted = (int)OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions;
                }
            }

            pros.Cleanup();
        }
    }
}
