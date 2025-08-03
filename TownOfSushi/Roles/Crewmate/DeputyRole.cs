﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class DeputyRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Deputy";
    public string RoleDescription => "Camp Crewmates To Catch Their Killer";
    public string RoleLongDescription => "Camp crewmates, then shoot their killer in the meeting!";
    public Color RoleColor => TownOfSushiColors.Deputy;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateKilling;
    public DoomableType DoomHintType => DoomableType.Relentless;
    public override bool IsAffectedByComms => false;
    public bool IsPowerCrew => Killer; // Only stop end game checks if the deputy can actually kill someone
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Deputy,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Impostor),
    };
    public static void OnRoundStart()
    {
        CustomButtonSingleton<CampButton>.Instance.Usable = true;
    }

    public PlayerControl? Killer { get; set; }

    private MeetingMenu meetingMenu;

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                ClickGuess,
                MeetingAbilityType.Click,
                TosAssets.ShootMeetingSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f),
            };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && Killer != null && !Player.HasModifier<JailedModifier>());
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
        var player = ModifierUtils.GetPlayersWithModifier<DeputyCampedModifier>(x => x.Deputy == PlayerControl.LocalPlayer).FirstOrDefault();

        if (player != null && Player.AmOwner)
        {
            player.RpcRemoveModifier<DeputyCampedModifier>();
        }
    }

    public void ClickGuess(PlayerVoteArea voteArea, MeetingHud __)
    {
        var target = GameData.Instance.GetPlayerById(voteArea.TargetPlayerId).Object;
        var role = Player.GetRole<DeputyRole>()!;

        if (role.Killer == target && !target.HasModifier<InvulnerabilityModifier>())
        {
            Player.RpcCustomMurder(target, createDeadBody: false, teleportMurderer: false);
        }
        else
        {
            var title = $"<color=#{TownOfSushiColors.Deputy.ToHtmlStringRGBA()}>Deputy Feedback</color>";
            MiscUtils.AddFakeChat(PlayerControl.LocalPlayer.Data, title, "You missed your shot! They are either not the killer or are invincible.", false, true);
            var notif1 = Helpers.CreateAndShowNotification($"<b>{TownOfSushiColors.Deputy.ToTextColor()}You missed your shot! They are either not the killer or are invincible.</b></color>", Color.white, new Vector3(0f, 1f, -20f), spr: TosRoleIcons.Deputy.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }

        if (Player.AmOwner)
        {
            meetingMenu?.HideButtons();
        }

        Clear();
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId == Player.PlayerId || Player.Data.IsDead || voteArea!.AmDead || voteArea.GetPlayer()?.HasModifier<JailedModifier>() == true;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    
    public string GetAdvancedDescription()
    {
        return "The Deputy is a Crewmate Killing role that can camp other players. Once a camped player dies the Deputy is alerted to their death. " +
               "The following meeting the Deputy may then attempt to shoot the killer of the camped player. If successful the killer dies and if not nothing happens." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Camp",
            $"Camp a player to be alerted once they die. After their death, you may attempt to shoot the killer. If your shot is successful, the killer dies, if not, nothing will happen.",
            TosCrewAssets.CampButtonSprite),
    ];
}
