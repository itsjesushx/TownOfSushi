﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ClericRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;
    public string RoleName => "Cleric";
    public string RoleDescription => "Save The Crewmates";
    public string RoleLongDescription => "Barrier and Cleanse crewmates";
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public Color RoleColor => TownOfSushiColors.Cleric;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Scientist),
        Icon = TOSRoleIcons.Cleric
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Cleric is a Crewmate Protective that can protect crewmates by negating their negative effects, as well as placing barriers on them to prevent interactions." +
            MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Barrier",
            $"Prevent a Crewmate from being interacted with. The shield will last for {OptionGroupSingleton<ClericOptions>.Instance.BarrierCooldown} seconds.",
            TOSCrewAssets.BarrierSprite),
        new("Cleanse",
            "Remove all negative effects on a player. (Douse (arso and pyro), Warlock spell, Hack, Infect, Blackmail, Blind, Flash, and Hypnosis)",
            TOSCrewAssets.CleanseSprite)
    ];

    [MethodRpc((uint)TownOfSushiRpc.ClericBarrierAttacked, SendImmediately = true)]
    public static void RpcClericBarrierAttacked(PlayerControl cleric, PlayerControl source, PlayerControl shielded)
    {
        if (cleric.Data.Role is not ClericRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcClericBarrierAttacked - Invalid cleric");
            return;
        }

        if (PlayerControl.LocalPlayer.PlayerId == source.PlayerId ||
            (PlayerControl.LocalPlayer.PlayerId == cleric.PlayerId &&
             OptionGroupSingleton<ClericOptions>.Instance.AttackNotif))
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Cleric));
        }
    }
}