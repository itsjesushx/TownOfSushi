using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using UnityEngine;
using MiraAPI.Networking;

namespace TownOfSushi.Roles.Crewmate;

public sealed class CrusaderRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IMysticClue
{
    public override bool IsAffectedByComms => false;

    public PlayerControl? Fortified { get; set; }
    public MysticClueType MysticHintType => MysticClueType.Protective;

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not CrusaderRole)
        {
            return;
        }

        if (Fortified != null && Fortified.HasDied())
        {
            Clear();
        }
    }
    public string RoleName => "Crusader";
    public string RoleDescription => "Fortify Crewmates";
    public string RoleLongDescription => "Fortify crewmates to prevent any interactions with them";
    public Color RoleColor => TownOfSushiColors.Crusader;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateProtective;

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TOSAudio.AdministratorIntroSound,
        Icon = TOSRoleIcons.Crusader
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);

        if (Fortified != null)
        {
            stringB.Append(CultureInfo.InvariantCulture,
                $"\n<b>Fortified: </b>{Color.white.ToTextColor()}{Fortified.Data.PlayerName}</color>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Crusader is a Crewmate Protective role that can fortify players to prevent them from being interacted with. "
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Fortify",
            "Fortify a player to prevent them from being interacted with. If anyone tries to interact with a fortified player, the ability will not work and both the Crusader and fortified player will be alerted with a purple flash.",
            TOSCrewAssets.FortifySprite)
    ];

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

    public void SetFortifiedPlayer(PlayerControl? player)
    {
        Fortified?.RemoveModifier<CrusaderFortifiedModifier>();

        Fortified = player;

        Fortified?.AddModifier<CrusaderFortifiedModifier>(Player);
    }

    [MethodRpc((uint)TownOfSushiRpc.CrusaderFortify, SendImmediately = true)]
    public static void RpcCrusaderFortify(PlayerControl player, PlayerControl target)
    {
        if (player.Data.Role is not CrusaderRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcCrusaderFortify - Invalid Crusader");
            return;
        }

        var Crusader = player.GetRole<CrusaderRole>();
        Crusader?.SetFortifiedPlayer(target);
    }

    [MethodRpc((uint)TownOfSushiRpc.CrusaderFortifyMurder, SendImmediately = true)]
    public static void RpcCrusaderFortifyMurder(PlayerControl target, PlayerControl attacker)
    {
        if (!target.HasModifier<CrusaderFortifiedModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("RpcCrusaderFortifyMurder - Source doesn't own fortified modifier");
            return;
        }

        target.RpcCustomMurder(attacker);

        if (attacker.AmOwner)
        {
            var notif = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Crusader,
                $"<b>{target.Data.PlayerName}, was fortified by a Crusader!</b>"),

                Color.white, spr: TOSRoleIcons.Crusader.LoadAsset());
                notif.AdjustNotification();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.ClearCrusaderFortify, SendImmediately = true)]
    public static void RpcClearCrusaderFortify(PlayerControl player)
    {
        if (player.Data.Role is not CrusaderRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcClearCrusaderFortify - Invalid Crusader");
            return;
        }

        var Crusader = player.GetRole<CrusaderRole>();
        Crusader?.SetFortifiedPlayer(null);
    }

    [MethodRpc((uint)TownOfSushiRpc.CrusaderNotify, SendImmediately = true)]
    public static void RpcCrusaderNotify(PlayerControl player, PlayerControl source, PlayerControl target)
    {
        if (player.Data.Role is not CrusaderRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcCrusaderNotify - Invalid Crusader");
            return;
        }

        // Logger<TownOfSushiPlugin>.Error("RpcCrusaderNotify");
        if (player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Crusader));
        }

        if (source.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Crusader));
        }
    }
}