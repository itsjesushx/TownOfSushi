﻿using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Meeting.Voting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public static class ExecutionerEvents
{
    [RegisterEvent(0)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason is DeathReason.Exile)
        {
            if (!@event.Player.TryGetModifier<ExecutionerTargetModifier>(out var exeMod))
            {
                return;
            }

            var exe = GameData.Instance.GetPlayerById(exeMod.OwnerId).Object;
            if (exe != null && !exe.HasDied() && exe.Data.Role is ExecutionerRole exeRole && exeRole.AboutToWin)
            {
                exeRole.TargetVoted = true;
            }
        }
        else
        {
            CustomRoleUtils.GetActiveRolesOfType<ExecutionerRole>().Do(x => x.CheckTargetDeath(@event.Player));
        }
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        foreach (var executioner in CustomRoleUtils.GetActiveRolesOfType<ExecutionerRole>())
        {
            if (!executioner.AboutToWin) executioner.Voters.Clear();
        }
        
        if (@event.TriggeredByIntro)
        {
            return;
        }

        if (OptionGroupSingleton<ExecutionerOptions>.Instance.ExeWin is ExeWinOptions.EndsGame)
        {
            return;
        }

        var exe = CustomRoleUtils.GetActiveRolesOfType<ExecutionerRole>()
            .FirstOrDefault(x => x.AboutToWin && !x.Player.HasDied());
        if (exe != null)
        {
            exe.TargetVoted = true;
            if (exe.Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Executioner,
                    $"<b>You have successfully won as the Executioner, as your target was exiled!</b>"),
                    Color.white, spr: TOSRoleIcons.Executioner.LoadAsset());

                
                notif1.AdjustNotification();

                PlayerControl.LocalPlayer.RpcPlayerExile();

                if (OptionGroupSingleton<ExecutionerOptions>.Instance.ExeWin is ExeWinOptions.Torments)
                {
                    CustomButtonSingleton<ExeTormentButton>.Instance.SetActive(true, exe);
                    DeathHandlerModifier.RpcUpdateDeathHandler(PlayerControl.LocalPlayer, "Victorious", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetTrue, lockInfo: DeathHandlerOverride.SetTrue);
                    var notif2 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Executioner,
                        $"<b>You have one round to torment a player of your choice to death, choose wisely.</b>"),
                        Color.white);

                    
                    notif2.transform.localPosition = new Vector3(0f, 0.85f, -20f);
                }
                else
                {
                    DeathHandlerModifier.RpcUpdateDeathHandler(PlayerControl.LocalPlayer, "Victorious", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                    PlayerControl.LocalPlayer.RpcRemoveModifier<IndirectAttackerModifier>();
                }
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Executioner,
                    $"<b>The Executioner, {exe.Player.Data.PlayerName}, has successfully won, as their target was exiled!</b>"),
                    Color.white, spr: TOSRoleIcons.Executioner.LoadAsset());

                
                notif1.AdjustNotification();
            }
        }
    }
    
    [RegisterEvent]
    public static void HandleVoteEventHandler(HandleVoteEvent @event)
    {
        var votingPlayer = @event.Player;
        var suspectPlayer = @event.TargetPlayerInfo;
        
        if (suspectPlayer == null || !suspectPlayer._object.TryGetModifier<ExecutionerTargetModifier>(out var exeMod))
        {
            return;
        }

        var exe = GameData.Instance.GetPlayerById(exeMod.OwnerId).Object;
        if (exe != null && !exe.HasDied() && exe.Data.Role is ExecutionerRole exeRole)
        {
            exeRole.Voters.Add(votingPlayer.PlayerId);
        }
    }
 
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;

        if (exiled == null || !exiled.TryGetModifier<ExecutionerTargetModifier>(out var exeMod))
        {
            return;
        }
        
        var exe = GameData.Instance.GetPlayerById(exeMod.OwnerId).Object;
        if (exe != null && !exe.HasDied() && exe.Data.Role is ExecutionerRole exeRole)
        {
            exeRole.AboutToWin = true;
            if (!PlayerControl.LocalPlayer.IsHost())
            {
                exeRole.TargetVoted = true;
            }
            
            if (exe.AmOwner && OptionGroupSingleton<ExecutionerOptions>.Instance.ExeWin is ExeWinOptions.Torments)
            {
                var allVoters = PlayerControl.AllPlayerControls.ToArray()
                    .Where(x => exeRole.Voters.Contains(x.PlayerId) && !x.AmOwner);
                
                if (!allVoters.Any())
                {
                    return;
                }

                foreach (var player in allVoters)
                {
                    player.AddModifier<MisfortuneTargetModifier>();
                }

                CustomButtonSingleton<ExeTormentButton>.Instance.Show = true;
            }
        }
    }
}