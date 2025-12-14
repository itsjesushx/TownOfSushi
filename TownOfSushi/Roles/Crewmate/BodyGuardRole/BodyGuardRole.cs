
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using MiraAPI.Networking;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyguardRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;

    public PlayerControl? Guarded { get; set; }
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not BodyguardRole)
        {
            return;
        }

        if (Guarded != null && Guarded.HasDied())
        {
            Clear();
        }
    }
    public string RoleName => "Bodyguard";
    public string RoleDescription => "Guard Crewmates to prevent their death";
    public string RoleLongDescription => "Guard crewmates to prevent any interactions with them";
    public Color RoleColor => TownOfSushiColors.Bodyguard;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.WarlockCurse,
        Icon = TOSRoleIcons.Bodyguard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Guarded != null)
        {
            stringB.Append(TownOfSushiPlugin.Culture,
                $"\n<b>Guarded: </b>{Color.white.ToTextColor()}{Guarded.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Bodyguard is a Crewmate Protective role that can guard players to prevent them from being interacted with. If the interaction counts as killing, the interacter and Bodyguard will die along. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Guard",
            "Guard a player to prevent them from being interacted with. If anyone tries to interact with a Guarded player, the ability will not work. If the interaction counts as killing, the interacter and Bodyguard will die along.",
            TOSRoleIcons.Bodyguard)
    ];

    public void Clear()
    {
        SetGuardedPlayer(null);
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
    }

    public void SetGuardedPlayer(PlayerControl? player)
    {
        Guarded?.RemoveModifier<BodyguardGuardedModifier>();

        Guarded = player;

        Guarded?.AddModifier<BodyguardGuardedModifier>(Player);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyguardGuard, SendImmediately = true)]
    public static void RpcBodyguardGuard(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not BodyguardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyguardGuard - Invalid Bodyguard");
            return;
        }

        var Bodyguard = player.GetRole<BodyguardRole>();
        Bodyguard?.SetGuardedPlayer(target);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyguardGuardMurder, SendImmediately = true)]
    public static void RpcBodyguardGuardMurder(PlayerControl bodyGuardPlayer, PlayerControl target, PlayerControl attacker)
    {
        if (!target.HasModifier<BodyguardGuardedModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyguardGuardMurder - Source doesn't own Guarded modifier");
            return;
        }
        if (bodyGuardPlayer.Data.Role is not BodyguardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyguardGuardMurder - Invalid Bodyguard");
            return;
        }

        bodyGuardPlayer.RpcCustomMurder(attacker);
        bodyGuardPlayer.RpcCustomMurder(bodyGuardPlayer);
        DeathHandlerModifier.RpcUpdateDeathHandler(bodyGuardPlayer, "Killed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {attacker.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);

        if (attacker.AmOwner)
        {
            var notif = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Bodyguard,
                $"<b>{target.Data.PlayerName}, was protected by a {bodyGuardPlayer.Data.PlayerName} (The Bodyguard)! They died with you as well!</b>"),

                Color.white, spr: TOSRoleIcons.Bodyguard.LoadAsset());
            notif.AdjustNotification();
        }
        if (bodyGuardPlayer.AmOwner)
        {
            var notif = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Bodyguard,
                $"<b>{target.Data.PlayerName}, your protectee, has survived thanks to you! you died protecting them</b>"),

                Color.white, spr: TOSRoleIcons.Bodyguard.LoadAsset());
                notif.AdjustNotification();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.ClearBodyguardGuard, SendImmediately = true)]
    public static void RpcClearBodyguardGuard(PlayerControl player)
    {
        if (player.Data.Role is not BodyguardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcClearBodyguardGuard - Invalid Bodyguard");
            return;
        }

        var Bodyguard = player.GetRole<BodyguardRole>();
        Bodyguard?.SetGuardedPlayer(null);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyguardNotify, SendImmediately = true)]
    public static void RpcBodyguardNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (player.Data.Role is not BodyguardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyguardNotify - Invalid Bodyguard");
            return;
        }

        // Logger<TownOfSushiPlugin>.Error("RpcBodyguardNotify");
        if (player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Bodyguard));
        }

        if (source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Bodyguard));
        }
    }
}