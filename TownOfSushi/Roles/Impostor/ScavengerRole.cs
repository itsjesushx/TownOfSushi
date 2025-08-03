﻿using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities;
using System.Collections;
using System.Globalization;
using System.Text;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Impostor;

using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class ScavengerRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Scavenger";
    public string RoleDescription => "Hunt Down Your Prey";
    public string RoleLongDescription => "Kill your given targets for a reduced kill cooldown";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TrackerTouRole>());
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public DoomableType DoomHintType => DoomableType.Hunter;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Scavenger,
        IntroSound = TosAudio.WarlockIntroSound,
    };

    public bool GameStarted { get; set; }
    public float TimeRemaining { get; set; }
    public PlayerControl? Target { get; set; }
    public bool Scavenging { get; set; }

    public override void OnDeath(DeathReason reason)
    {
        RoleBehaviourStubs.OnDeath(this, reason);

        Clear();
    }
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (TutorialManager.InstanceExists && Target == null && Player.AmOwner)
        {
            Coroutines.Start(SetTutorialTarget(this, Player));
        }
    }
    private static IEnumerator SetTutorialTarget(ScavengerRole scav, PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);
        scav.GameStarted = true;
        scav.Scavenging = false;
        if (player.killTimer <= 0f && !player.HasDied())
        {
            // Logger<TownOfSushiPlugin>.Message($"Scavenge Begin");
            scav.Scavenging = true;
            scav.TimeRemaining = OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeDuration;

            scav.Target = player.GetClosestLivingPlayer(false, float.MaxValue, true, predicate: x => !x.HasModifier<FirstDeadShield>())!;

            scav.Target.AddModifier<ScavengerArrowModifier>(player, TownOfSushiColors.Impostor);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        Clear();
    }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not ScavengerRole) return;
        if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started && !TutorialManager.InstanceExists) return;
        if (!Player.AmOwner) return;

        if (!GameStarted && Player.killTimer > 0f) GameStarted = true;

        // scavenge mode starts once kill timer reaches 0
        if (Player.killTimer <= 0f && !Scavenging && GameStarted && !Player.HasDied())
        {
            // Logger<TownOfSushiPlugin>.Message($"Scavenge Begin");
            Scavenging = true;
            TimeRemaining = OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeDuration;

            Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true, predicate: x => !x.HasModifier<FirstDeadShield>())!;

            Target.AddModifier<ScavengerArrowModifier>(Player, TownOfSushiColors.Impostor);
        }

        if (TimeRemaining > 0)
            TimeRemaining -= Time.deltaTime;

        if ((TimeRemaining <= 0 || MeetingHud.Instance || Player.HasDied()) && Scavenging)
        {
            Clear();

            // Logger<TownOfSushiPlugin>.Message($"Scavenge End");
            Player.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
        }
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Target != null && Scavenging)
        {
            stringB.Append("\n<b>Scavenge Time:</b> ");
            stringB.Append(CultureInfo.InvariantCulture, $"{Color.white.ToTextColor()}{TimeRemaining.ToString("0", CultureInfo.InvariantCulture)}</color>");
            stringB.Append("\n\n<b>Current Target:</b> ");
            stringB.Append(CultureInfo.InvariantCulture, $"{Color.white.ToTextColor()}{Target.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public void Clear()
    {
        var players = ModifierUtils.GetPlayersWithModifier<ScavengerArrowModifier>();

        foreach (var player in players)
        {
            player.RemoveModifier<ScavengerArrowModifier>();
        }

        Scavenging = false;
        TimeRemaining = 0;
        Target = null;
    }

    public void OnPlayerKilled(PlayerControl victim)
    {
        if (!Player.AmOwner) return;

        if (victim == Target)
        {
            // extend scavenge duration
            TimeRemaining += OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeIncreaseDuration;

            // set kill timer
            Player.SetKillTimer(OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeCorrectKillCooldown);

            // get new target
            Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true)!;

            // update arrow to point to new target
            Target.AddModifier<ScavengerArrowModifier>(Player, TownOfSushiColors.Impostor);
        }
        else
        {
            // set kill timer
            Player.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() * OptionGroupSingleton<ScavengerOptions>.Instance.ScavengeIncorrectKillCooldown);

            // clear arrows
            Clear();
        }
    }
    public string GetAdvancedDescription()
    {
        return $"The Scavenger is an Impostor Killing role that gets new targets after every kill and when the round starts. "
            + "If they kill their target, they get a reduced kill cooldown, but if they don't, their cooldown is increased significantly."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
