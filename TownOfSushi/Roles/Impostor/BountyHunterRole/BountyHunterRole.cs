using System.Collections;
using System.Globalization;
using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using InnerNet;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class BountyHunterRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public bool GameStarted { get; set; }
    public float TimeRemaining { get; set; }
    [HideFromIl2Cpp]
    public PlayerControl? Target { get; set; }
    public bool Scavenging { get; set; }
    public MysticClueType MysticHintType => MysticClueType.Hunter;

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not BountyHunterRole)
        {
            return;
        }

        if (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started &&
            !TutorialManager.InstanceExists)
        {
            return;
        }

        if (!Player.AmOwner)
        {
            return;
        }

        if (MeetingHud.Instance || ExileController.Instance)
        {
            Scavenging = false;
            GameStarted = false;
            return;
        }

        if (!GameStarted && Player.killTimer > 0f)
        {
            GameStarted = true;
        }

        // scavenge mode starts once kill timer reaches 0
        if (Player.killTimer <= 0f && !Scavenging && GameStarted && !Player.HasDied())
        {
            // Logger<TownOfSushiPlugin>.Message($"Scavenge Begin");
            Scavenging = true;
            TimeRemaining = OptionGroupSingleton<BountyHunterOptions>.Instance.ScavengeDuration;

            Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true,
                x => !x.HasModifier<FirstDeadShield>())!;

            if (Player.HasModifier<LoverModifier>())
            {
                Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true,
                    x => !x.HasModifier<FirstDeadShield>() && !x.HasModifier<LoverModifier>())!;
            }

            Target.AddModifier<BountyHunterArrowModifier>(Player, TownOfSushiColors.Impostor);
        }

        if (TimeRemaining > 0)
        {
            TimeRemaining -= Time.deltaTime;
        }

        if ((TimeRemaining <= 0 || MeetingHud.Instance || Player.HasDied()) && Scavenging)
        {
            Clear();

            // Logger<TownOfSushiPlugin>.Message($"Scavenge End");
            Player.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
        }
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TrackerTOSRole>());
    public string RoleName => "Bounty Hunter";
    public string RoleDescription => "Hunt Down Your Prey";
    public string RoleLongDescription => "Kill your given targets for a reduced kill cooldown";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.BountyHunter,
        IntroSound = TOSAudio.HexbladeIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Target != null && Scavenging)
        {
            stringB.Append("\n<b>Scavenge Time:</b> ");
            stringB.Append(CultureInfo.InvariantCulture,
                $"{Color.white.ToTextColor()}{TimeRemaining.ToString("0", CultureInfo.InvariantCulture)}</color>");
            stringB.Append("\n\n<b>Current Target:</b> ");
            stringB.Append(CultureInfo.InvariantCulture,
                $"{Color.white.ToTextColor()}{Target.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The BountyHunter is an Impostor Killing role that gets new targets after every kill and when the round starts. "
            + "If they kill their target, they get a reduced kill cooldown, but if they don't, their cooldown is increased significantly."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp] public List<CustomButtonWikiDescription> Abilities { get; } = [];

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

    private static IEnumerator SetTutorialTarget(BountyHunterRole scav, PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);
        scav.GameStarted = true;
        scav.Scavenging = false;
        if (player.killTimer <= 0f && !player.HasDied())
        {
            // Logger<TownOfSushiPlugin>.Message($"Scavenge Begin");
            scav.Scavenging = true;
            scav.TimeRemaining = OptionGroupSingleton<BountyHunterOptions>.Instance.ScavengeDuration;

            scav.Target =
                player.GetClosestLivingPlayer(false, float.MaxValue, true, x => !x.HasModifier<FirstDeadShield>())!;
            
            if (player.HasModifier<LoverModifier>())
            {
                scav.Target = player.GetClosestLivingPlayer(false, float.MaxValue, true,
                    x => !x.HasModifier<FirstDeadShield>() && !x.HasModifier<LoverModifier>())!;
            }

            scav.Target.AddModifier<BountyHunterArrowModifier>(player, TownOfSushiColors.Impostor);
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        Clear();
    }

    public void Clear()
    {
        var players = ModifierUtils.GetPlayersWithModifier<BountyHunterArrowModifier>();

        foreach (var player in players)
        {
            player.RemoveModifier<BountyHunterArrowModifier>();
        }

        Scavenging = false;
        TimeRemaining = 0;
        Target = null;
    }

    public void OnPlayerKilled(PlayerControl victim)
    {
        if (!Player.AmOwner)
        {
            return;
        }

        if (victim == Target)
        {
            // extend scavenge duration
            TimeRemaining += OptionGroupSingleton<BountyHunterOptions>.Instance.ScavengeIncreaseDuration;

            // set kill timer
            Player.SetKillTimer(OptionGroupSingleton<BountyHunterOptions>.Instance.ScavengeCorrectKillCooldown);

            // get new target
            Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true)!;
            
            if (Player.HasModifier<LoverModifier>())
            {
                Target = Player.GetClosestLivingPlayer(false, float.MaxValue, true,
                    x => !x.HasModifier<FirstDeadShield>() && !x.HasModifier<LoverModifier>())!;
            }

            // update arrow to point to new target
            Target.AddModifier<BountyHunterArrowModifier>(Player, TownOfSushiColors.Impostor);
        }
        else
        {
            // set kill timer
            Player.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown() *
                                OptionGroupSingleton<BountyHunterOptions>.Instance.ScavengeIncorrectKillCooldown);

            // clear arrows
            Clear();
        }
    }
}