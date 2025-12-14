using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class HypnotistRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    private MeetingMenu meetingMenu;

    public bool HysteriaActive { get; set; }
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not HypnotistRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<HypnotistOptions>.Instance.HypnoKill ||
                                                     (Player != null && Player.GetModifiers<BaseModifier>()
                                                         .Any(x => x is ICachedRole)) ||
                                                     (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<LookoutRole>());
    public string RoleName => "Hypnotist";
    public string RoleDescription => "Hypnotize players and make them go insane";
    public string RoleLongDescription => "Hypnotize crewmates and drive them insane";
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        Icon = TOSRoleIcons.Hypnotist
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Hypnotist is an Impostor Support role that can hypnotize players. During a meeting they can release Mass Hysteria, which makes all hypnotized players (marked with <color=#D53F42>@</color>) have different visuals applied to players the following round."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Hypnotize",
            "Hypnotize a player, causing them to see the game differently than non-hypnotized players if mass hysteria is active.",
            TOSImpAssets.HypnotiseButtonSprite),
        new("Mass Hysteria (Meeting)",
            "Cause all hypnotised players to have different visuals applied to players on their screen the following round.",
            TOSAssets.HysteriaCleanSprite)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                Click,
                MeetingAbilityType.Click,
                TOSAssets.HysteriaSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.40f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Player.AmOwner)
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !HysteriaActive && !Player.HasModifier<JailedModifier>());
        }
    }

    public override void OnVotingComplete()
    {
        RoleBehaviourStubs.OnVotingComplete(this);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        HysteriaActive = false;

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void Click(PlayerVoteArea voteArea, MeetingHud __)
    {
        RpcHysteria(Player);

        if (Player.AmOwner)
        {
            meetingMenu.HideButtons();
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId;
    }

    [MethodRpc((uint)TownOfSushiRpc.Hysteria, SendImmediately = true)]
    public static void RpcHysteria(PlayerControl player)
    {
        if (player.Data.Role is not HypnotistRole)
        {
            Logger<TownOfSushiPlugin>.Error("RpcHysteria - Invalid hypnotist");
            return;
        }

        var role = player.GetRole<HypnotistRole>();
        role!.HysteriaActive = true;
    }
}