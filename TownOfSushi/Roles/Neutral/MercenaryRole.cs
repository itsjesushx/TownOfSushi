﻿using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using System.Globalization;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Roles.Neutral;

public sealed class MercenaryRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Mercenary";
    public string RoleDescription => "Bribe The Crewmates";
    public string RoleLongDescription => "Guard crewmates, and then bribe the winners!";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<WardenRole>());
    public Color RoleColor => TownOfSushiColors.Mercenary;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralBenign;
    public DoomableType DoomHintType => DoomableType.Insight;
    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TosAudio.ToppatIntroSound,
        Icon = TosRoleIcons.Mercenary,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };

    public static int BrideCost => (int)OptionGroupSingleton<MercenaryOptions>.Instance.BribeCost;

    public int Gold { get; set; }
    public bool CanBribe => Gold >= BrideCost;

    public override bool DidWin(GameOverReason gameOverReason)
    {
        var bribed = ModifierUtils.GetPlayersWithModifier<MercenaryBribedModifier>();

        return bribed.Any(x => x.Data.Role.DidWin(gameOverReason));
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        var players = ModifierUtils.GetPlayersWithModifier<MercenaryBribedModifier>();

        stringB.Append(CultureInfo.InvariantCulture, $"\n<b>Gold:</b> {Gold}");

        var playerControls = players as PlayerControl[] ?? [.. players];
        if (playerControls.Length != 0)
        {
            stringB.Append($"\n<b>Bribed:</b>");
        }

        foreach (var player in playerControls)
        {
            stringB.Append(CultureInfo.InvariantCulture, $"\n{player.Data.PlayerName}");
        }

        return stringB;
    }

    public void AddPayment()
    {
        Gold++;

        if (CanBribe)
        {
            CustomButtonSingleton<MercenaryBribeButton>.Instance.SetActive(true, this);
        }
    }

    public void Clear()
    {
        Gold = 0;

        CustomButtonSingleton<MercenaryGuardButton>.Instance.SetActive(true, this);
        CustomButtonSingleton<MercenaryBribeButton>.Instance.SetActive(false, this);
    }

    [MethodRpc((uint)TownOfSushiRpc.Guarded, SendImmediately = true)]
    public static void RpcGuarded(PlayerControl player)
    {
        if (player.Data.Role is not MercenaryRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcGuarded - Invalid mercenary");
            return;
        }

        if (!player.AmOwner) return;

        var mercenary = player.GetRole<MercenaryRole>();
        mercenary?.AddPayment();
    }
    public string GetAdvancedDescription()
    {
        return
            "The Mercenary is a Neutral Evil role that can only win by bribing players, allowing them to gain multiple win conditions."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Guard",
            $"Guarding a player allows the Mercenary to absorb an ability used on the target. This will grant them gold, for Bribing. If any bribed targets win, the Mercenary will win with them.",
            TosNeutAssets.GuardSprite),
        new("Bribe",
            $"Bribing a player allows the Mercenary to gain their win condition, given that they have gold to spare.",
            TosNeutAssets.BribeSprite),
    ];
}
