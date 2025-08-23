﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TrapperRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public override bool IsAffectedByComms => false;

    [HideFromIl2Cpp] public List<RoleBehaviour> TrappedPlayers { get; set; } = new();

    public string RoleName => "Trapper";
    public string RoleDescription => "Catch Killers In The Act";
    public string RoleLongDescription => "Place traps around the map, revealing roles within them";
    public Color RoleColor => TownOfSushiColors.Trapper;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Trapper,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Tracker)
    };

    public void LobbyStart()
    {
        Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return $"The {RoleName} is a Crewmate Investigative role that can place traps around the map. " +
               "If someone stays in it for enough time and enough players go through, " +
               "they will get a list of their roles in the next meeting in random order." +
               MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Trap",
            "Places a trap. Depending on settings they may stay the entire game or reset after meetings.",
            TOSCrewAssets.TrapSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        Clear();
    }

    public void Clear()
    {
        TrappedPlayers.Clear();
        Trap.Clear();
    }

    public void Report()
    {
        // Logger<TownOfSushiPlugin>.Error($"TrapperRole.Report");
        if (!Player.AmOwner)
        {
            return;
        }

        var minAmountOfPlayersInTrap = OptionGroupSingleton<TrapperOptions>.Instance.MinAmountOfPlayersInTrap;
        var msg = "No players entered any of your traps";

        if (TrappedPlayers.Count < minAmountOfPlayersInTrap)
        {
            msg = "Not enough players triggered your traps";
        }
        else if (TrappedPlayers.Count != 0)
        {
            var message = new StringBuilder("Roles caught in your trap:\n");

            TrappedPlayers.Shuffle();

            foreach (var role in TrappedPlayers)
            {
                message.Append(TownOfSushiPlugin.Culture, $"{role.NiceName}, ");
            }

            message = message.Remove(message.Length - 2, 2);

            var finalMessage = message.ToString();

            if (string.IsNullOrWhiteSpace(finalMessage))
            {
                return;
            }

            msg = finalMessage;
        }

        var title = $"<color=#{TownOfSushiColors.Trapper.ToHtmlStringRGBA()}>{RoleName} Report</color>";
        MiscUtils.AddFakeChat(Player.Data, title, msg, false, true);
    }
}