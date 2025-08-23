﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Networking;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class DeputyRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable
{
    private MeetingMenu meetingMenu;
    public string RoleName => "Deputy";
    public string RoleDescription => "Execute killers mid-meeting!";
    public string RoleLongDescription => "Execute suspicious players.";
    public Color RoleColor => TownOfSushiColors.Deputy;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public bool IsPowerCrew => true;
    public int MissedShots { get; set; }

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Deputy,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} can shoot any player mid meeting, if they are evil, they die. If not, the {RoleName} loses half their vision and won't be able to execute until after the next next meeting." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                TOSAssets.ShootMeetingSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }

        Clear();
    }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        Clear();
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void Clear()
    {
        var player = ModifierUtils.GetPlayersWithModifier<DeputyCampedModifier>(x => x.Deputy.AmOwner).FirstOrDefault();

        if (player != null && Player.AmOwner)
        {
            player.RpcRemoveModifier<DeputyCampedModifier>();
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);
        
        if (Player.HasModifier<DeputyLowVisionModifier>())
        {
            Player.RemoveModifier<DeputyLowVisionModifier>();
        }

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;

        if (target.IsKillerRole() && !target.IsProtected())
        {
            Player.RpcCustomMurder(target, createDeadBody: false, teleportMurderer: false);
        }
        else if (target.IsPassiveNeutral() && !target.IsProtected())
        {
            Player.RpcCustomMurder(target, createDeadBody: false, teleportMurderer: false);
        }
        else
        {
            MissedShots++;

            if (MissedShots == 1)
            {
                // First miss: reduce vision
                var title = $"<color=#{TownOfSushiColors.Deputy.ToHtmlStringRGBA()}>Deputy Feedback</color>";
                MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title,
                    "You missed your shot! You lost half your vision. Next time you miss you will die.", false, true);

                var notif1 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfSushiColors.Deputy.ToTextColor()}You missed your shot! You lost half your vision. Next time you miss you will die.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Deputy.LoadAsset());
                notif1.Text.SetOutlineThickness(0.35f);

                // Apply vision penalty using a modifier
                Player.AddModifier<DeputyLowVisionModifier>();
            }
            else
            {
                // Second miss: suicide
                var notif2 = Helpers.CreateAndShowNotification(
                    $"<b>{TownOfSushiColors.Deputy.ToTextColor()}You missed again... and paid the price.</b>",
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Deputy.LoadAsset());
                notif2.Text.SetOutlineThickness(0.35f);

                Player.RpcCustomMurder(Player, createDeadBody: false, teleportMurderer: false);
            }
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }

        Clear();
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead ||
        (voteArea.GetPlayer()?.Data.Role is MonarchRole && Player.HasModifier<MonarchKnightedModifier>()) ||
               voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true;
    }
}