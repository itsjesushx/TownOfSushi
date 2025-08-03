using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modules.Wiki;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class WardenRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Warden";
    public string RoleDescription => "Fortify Crewmates";
    public string RoleLongDescription => "Fortify crewmates to prevent interactions with them";
    public Color RoleColor => TownOfSushiColors.Warden;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;
    public DoomableType DoomHintType => DoomableType.Protective;
    public override bool IsAffectedByComms => false;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TosAudio.SpyIntroSound,
        Icon = TosRoleIcons.Warden,
    };

    public PlayerControl? Fortified { get; set; }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Fortified != null)
        {
            stringB.Append(CultureInfo.InvariantCulture, $"\n<b>Fortified: </b>{Color.white.ToTextColor()}{Fortified.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public void Clear()
    {
        SetFortifiedPlayer(null);
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

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not WardenRole) return;
        if (Fortified != null && Fortified.HasDied())
            Clear();
    }

    public void SetFortifiedPlayer(PlayerControl? player)
    {
        Fortified?.RemoveModifier<WardenFortifiedModifier>();

        Fortified = player;

        Fortified?.AddModifier<WardenFortifiedModifier>(Player);
    }

    [MethodRpc((uint)TownOfSushiRpc.WardenFortify, SendImmediately = true)]
    public static void RpcWardenFortify(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcWardenFortify - Invalid warden");
            return;
        }

        var warden = player.GetRole<WardenRole>();
        warden?.SetFortifiedPlayer(target);
    }

    [MethodRpc((uint)TownOfSushiRpc.ClearWardenFortify, SendImmediately = true)]
    public static void RpcClearWardenFortify(PlayerControl player)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcClearWardenFortify - Invalid warden");
            return;
        }

        var warden = player.GetRole<WardenRole>();
        warden?.SetFortifiedPlayer(null);
    }

    [MethodRpc((uint)TownOfSushiRpc.WardenNotify, SendImmediately = true)]
    public static void RpcWardenNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (player.Data.Role is not WardenRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcWardenNotify - Invalid warden");
            return;
        }

        // Logger<TownOfSushiPlugin>.Error("RpcWardenNotify");
        if (player.AmOwner)
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Warden));

        if (source.AmOwner)
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Warden));
    }
    
    public string GetAdvancedDescription()
    {
        return
            "The Warden is a Crewmate Protective role that can fortify players to prevent them from being interacted with. "
            + MiscUtils.AppendOptionsText(GetType());
    }
    
    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Fortify",
            $"Fortify a player to prevent them from being interacted with. If anyone tries to interact with a fortified player, the ability will not work and both the Warden and fortified player will be alerted with a purple flash.",
            TosCrewAssets.FortifySprite),
    ];
}
