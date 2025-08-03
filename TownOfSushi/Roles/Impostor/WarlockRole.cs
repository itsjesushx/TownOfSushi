﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Roles;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class WarlockRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Warlock";
    public string RoleDescription => "Charge Up Your Kill Button To Multi Kill";
    public string RoleLongDescription => "Kill people in small bursts";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public DoomableType DoomHintType => DoomableType.Relentless;
    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = false,
        IntroSound = TosAudio.WarlockIntroSound,
        Icon = TosRoleIcons.Warlock,
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }
    public string GetAdvancedDescription()
    {
        return
            "The Warlock is an Impostor Killing role that can charge up attacks to wipe out the crew quickly."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Kill",
            $"Replaces your regular kill button with three stages: On Cooldown, Uncharged, and Charged. " +
            "You cannot kill while on cooldown but can while it is charging up, however it will reset your charge. " +
            "When it is charged, you can kill in a small burst to kill multiple players in a short time.",
            TosAssets.KillSprite),
    ];
}