﻿using System.Globalization;
using System.Text;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;

using TownOfSushi.Utilities;
using UnityEngine;
using UnityEngine.Events;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ProsecutorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Prosecutor";
    public string RoleDescription => "Exile Players Of Your Choosing";
    public string RoleLongDescription => "Choose to exile anyone you want";
    public Color RoleColor => TownOfSushiColors.Prosecutor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;
    public DoomableType DoomHintType => DoomableType.Fearmonger;
    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 1,
        Icon = TosRoleIcons.Prosecutor,
        IntroSound = TosAudio.ProsIntroSound,
    };

    public PlayerVoteArea? ProsecuteButton { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the Prosecutor has selected a victim.
    /// </summary>
    public bool HasProsecuted { get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether the Prosecutor has pressed the Prosecute button and is selecting a victim.
    /// </summary>
    public bool SelectingProsecuteVictim { get; set; }

    /// <summary>
    /// Gets or sets a value indicating how many prosecutions have been completed.
    /// </summary>
    public int ProsecutionsCompleted { get; set; }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.HasModifier<ImitatorCacheModifier>()) ProsecutionsCompleted = (int)OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions;
    }
    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var text = ITownOfSushiRole.SetNewTabText(this);
        if (PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            text.AppendLine(CultureInfo.InvariantCulture, $"<b>You may prosecute crew.</b>");
        }
        var prosecutes = OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions - ProsecutionsCompleted;
        var newText = prosecutes == 1 ? $"1 Prosecution Remaining." : $"\n{prosecutes} Prosecutions Remaining.";
        text.AppendLine(CultureInfo.InvariantCulture, $"{newText}");
        return text;
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        var meeting = MeetingHud.Instance;
        if (!Player.AmOwner || meeting == null || ProsecutionsCompleted >= OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions) return;

        var skip = meeting.SkipVoteButton;
        ProsecuteButton = Instantiate(skip, skip.transform.parent);
        ProsecuteButton.Parent = meeting;
        ProsecuteButton.SetTargetPlayerId(251);
        ProsecuteButton.transform.localPosition = skip.transform.localPosition + new Vector3(0f, -0.17f, 0f);

        ProsecuteButton.gameObject.GetComponentInChildren<TextTranslatorTMP>().Destroy();
        ProsecuteButton.gameObject.GetComponentInChildren<TextMeshPro>().text = "PROSECUTE";

        foreach (var plr in meeting.playerStates.AddItem(skip))
        {
            plr.gameObject.GetComponentInChildren<PassiveButton>().OnClick
                .AddListener((UnityAction)(() => ProsecuteButton.ClearButtons()));
        }

        skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
    }

    public void Cleanup()
    {
        ProsecuteButton = null;
        SelectingProsecuteVictim = false;

        if (HasProsecuted)
        {
            ProsecutionsCompleted++;
        }

        HasProsecuted = false;
    }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not ProsecutorRole) return;

        var meeting = MeetingHud.Instance;

        if (!Player.AmOwner || meeting == null || ProsecuteButton == null) return;

        ProsecuteButton.gameObject.SetActive(meeting.SkipVoteButton.gameObject.active && !SelectingProsecuteVictim);

        if (!ProsecuteButton.gameObject.active) return;

        if (meeting.state == MeetingHud.VoteStates.Discussion &&
            meeting.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
        {
            ProsecuteButton.SetDisabled();
        }
        else
        {
            ProsecuteButton.SetEnabled();
        }

        ProsecuteButton.voteComplete = meeting.SkipVoteButton.voteComplete;
    }

    [MethodRpc((uint)TownOfSushiRpc.Prosecute, SendImmediately = true)]
    public static void RpcProsecute(PlayerControl plr)
    {
        if (plr.Data.Role is not ProsecutorRole prosecutorRole)
        {
            return;
        }

        if (prosecutorRole.ProsecutionsCompleted >= OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions)
        {
            return;
        }

        prosecutorRole.HasProsecuted = true;
    }

    public string GetAdvancedDescription()
    {
        return "The Prosecutor is a Crewmate Power role that can Exile a player, applying 5 votes to a player of their choosing. They can also see who voted for who, even if they’re anonymous."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Prosecute (Meeting)",
            "Exile any player of your choosing, throwing 5 votes on them and ignoring all other votes.",
            TosRoleIcons.Prosecutor)
    ];
}
