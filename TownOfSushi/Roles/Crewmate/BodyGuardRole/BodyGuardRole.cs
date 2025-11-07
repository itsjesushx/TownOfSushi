using System.Globalization;
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

public sealed class BodyGuardRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;

    public PlayerControl? Guarded { get; set; }
    public MysticClueType MysticHintType => MysticClueType.Protective;
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not BodyGuardRole)
        {
            return;
        }

        if (Guarded != null && Guarded.HasDied())
        {
            Clear();
        }
    }
    public string RoleName => "BodyGuard";
    public string RoleDescription => "Guard Crewmates to prevent their death";
    public string RoleLongDescription => "Guard crewmates to prevent any interactions with them";
    public Color RoleColor => TownOfSushiColors.BodyGuard;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.WarlockCurse,
        Icon = TOSRoleIcons.BodyGuard
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Guarded != null)
        {
            stringB.Append(CultureInfo.InvariantCulture,
                $"\n<b>Guarded: </b>{Color.white.ToTextColor()}{Guarded.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The BodyGuard is a Crewmate Protective role that can guard players to prevent them from being interacted with. If the interaction counts as killing, the interacter and BodyGuard will die along. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Guard",
            "Guard a player to prevent them from being interacted with. If anyone tries to interact with a Guarded player, the ability will not work. If the interaction counts as killing, the interacter and BodyGuard will die along.",
            TOSRoleIcons.BodyGuard)
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
        Guarded?.RemoveModifier<BodyGuardGuardedModifier>();

        Guarded = player;

        Guarded?.AddModifier<BodyGuardGuardedModifier>(Player);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyGuardGuard, SendImmediately = true)]
    public static void RpcBodyGuardGuard(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not BodyGuardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyGuardGuard - Invalid BodyGuard");
            return;
        }

        var BodyGuard = player.GetRole<BodyGuardRole>();
        BodyGuard?.SetGuardedPlayer(target);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyGuardGuardMurder, SendImmediately = true)]
    public static void RpcBodyGuardGuardMurder(PlayerControl bodyGuardPlayer, PlayerControl target, PlayerControl attacker)
    {
        if (!target.HasModifier<BodyGuardGuardedModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyGuardGuardMurder - Source doesn't own Guarded modifier");
            return;
        }
        if (bodyGuardPlayer.Data.Role is not BodyGuardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyGuardGuardMurder - Invalid BodyGuard");
            return;
        }

        bodyGuardPlayer.RpcCustomMurder(attacker);
        bodyGuardPlayer.RpcCustomMurder(bodyGuardPlayer);
        DeathHandlerModifier.RpcUpdateDeathHandler(bodyGuardPlayer, "Killed", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, $"By {attacker.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);

        if (attacker.AmOwner)
        {
            var notif = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.BodyGuard,
                $"<b>{target.Data.PlayerName}, was protected by a BodyGuard! They died with you as well!</b>"),

                Color.white, spr: TOSRoleIcons.BodyGuard.LoadAsset());
            notif.AdjustNotification();
        }
        if (bodyGuardPlayer.AmOwner)
        {
            var notif = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.BodyGuard,
                $"<b>{target.Data.PlayerName}, your protectee, has survived thanks to you! you died protecting them</b>"),

                Color.white, spr: TOSRoleIcons.BodyGuard.LoadAsset());
                notif.AdjustNotification();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.ClearBodyGuardGuard, SendImmediately = true)]
    public static void RpcClearBodyGuardGuard(PlayerControl player)
    {
        if (player.Data.Role is not BodyGuardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcClearBodyGuardGuard - Invalid BodyGuard");
            return;
        }

        var BodyGuard = player.GetRole<BodyGuardRole>();
        BodyGuard?.SetGuardedPlayer(null);
    }

    [MethodRpc((uint)TownOfSushiRpc.BodyGuardNotify, SendImmediately = true)]
    public static void RpcBodyGuardNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (player.Data.Role is not BodyGuardRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcBodyGuardNotify - Invalid BodyGuard");
            return;
        }

        // Logger<TownOfSushiPlugin>.Error("RpcBodyGuardNotify");
        if (player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.BodyGuard));
        }

        if (source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.BodyGuard));
        }
    }
}