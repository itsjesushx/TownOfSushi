﻿using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrackerTOSRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Tracker";
    public string RoleDescription => "Track Everyone's Movement";
    public string RoleLongDescription => "Track suspicious players to see where they go";
    public MysticClueType MysticHintType => MysticClueType.Hunter;
    public Color RoleColor => TownOfSushiColors.Tracker;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Tracker,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Tracker)
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var players =
            ModifierUtils.GetPlayersWithModifier<TrackerArrowTargetModifier>([HideFromIl2Cpp](x) => x.Owner == Player);

        var playerControls = players as PlayerControl[] ?? players.ToArray();
        if (playerControls.Length == 0)
        {
            return stringB;
        }

        stringB.Append("\n<b>Tracked Players:</b>");
        foreach (var plr in playerControls)
        {
            stringB.Append(CultureInfo.InvariantCulture, $"\n{plr.Data.PlayerName}");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Tracker is a Crewmate Investigative role that can track other players to see their general position across the map, getting colored arrows towards all tracked players."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities =>
    [
        new("Track",
            "Track a player to see where they go." +
            "You will have an arrow pointing to their location that will update periodically." +
            "It will disappear if they die, or depending on settings, the trackers will be reset after a meeting.",
            TOSCrewAssets.TrackSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        var players =
            ModifierUtils.GetPlayersWithModifier<TrackerArrowTargetModifier>([HideFromIl2Cpp](x) => x.Owner == Player);

        foreach (var player in players)
        {
            player.RemoveModifier<TrackerArrowTargetModifier>();
        }
    }
}