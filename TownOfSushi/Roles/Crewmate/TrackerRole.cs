﻿using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules.Wiki;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrackerTouRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Tracker";
    public string RoleDescription => "Track Everyone's Movement";
    public string RoleLongDescription => "Track suspicious players to see where they go";
    public Color RoleColor => TownOfSushiColors.Tracker;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;
    public DoomableType DoomHintType => DoomableType.Hunter;
    public override bool IsAffectedByComms => false;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Tracker,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Tracker),
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        var players = ModifierUtils.GetPlayersWithModifier<TrackerArrowTargetModifier>([HideFromIl2Cpp] (x) => x.Owner == Player);

        var playerControls = players as PlayerControl[] ?? players.ToArray();
        if (playerControls.Length == 0) return stringB;
        stringB.Append("\n<b>Tracked Players:</b>");
        foreach (var plr in playerControls)
        {
            stringB.Append(CultureInfo.InvariantCulture, $"\n{plr.Data.PlayerName}");
        }

        return stringB;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        var players = ModifierUtils.GetPlayersWithModifier<TrackerArrowTargetModifier>([HideFromIl2Cpp] (x) => x.Owner == Player);

        foreach (var player in players)
        {
            player.RemoveModifier<TrackerArrowTargetModifier>();
        }
    }
    public string GetAdvancedDescription()
    {
        return
            "The Tracker is a Crewmate Investigative role that can track other players to see their general position across the map, getting colored arrows towards all tracked players."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities => [
        new("Track",
            "Track a player to see where they go." +
            "You will have an arrow pointing to their location that will update periodically." +
            "It will disappear if they die, or depending on settings, the trackers will be reset after a meeting.",
            TosCrewAssets.TrackSprite),
    ];
}
