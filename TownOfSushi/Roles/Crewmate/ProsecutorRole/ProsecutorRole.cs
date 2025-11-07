
using System.Text;
using HarmonyLib;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Utilities.Extensions;
using TMPro;
using TownOfSushi.Modifiers.Game;
using UnityEngine;
using UnityEngine.Events;

namespace TownOfSushi.Roles.Crewmate;

public sealed class ProsecutorRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable, IMysticClue
{
    [HideFromIl2Cpp]
    public PlayerVoteArea? ProsecuteButton { get; private set; }
    public MysticClueType MysticHintType => MysticClueType.Fearmonger;
    public bool HasProsecuted { get; private set; }
    public byte ProsecuteVictim { get; set; } = byte.MaxValue;
    public bool HideProsButton { get; set; }
    public bool SelectingProsecuteVictim { get; set; }
    public int ProsecutionsCompleted { get; set; }

    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not ProsecutorRole)
        {
            return;
        }

        var meeting = MeetingHud.Instance;

        if (!Player.AmOwner || meeting == null || ProsecuteButton == null)
        {
            return;
        }

        ProsecuteButton.gameObject.SetActive(!HideProsButton && meeting.state == MeetingHud.VoteStates.NotVoted && !SelectingProsecuteVictim);

        if (!ProsecuteButton.gameObject.active)
        {
            return;
        }

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
    public string RoleName => "Prosecutor";
    public string RoleDescription => "Exile Players Of Your Choosing";
    public string RoleLongDescription => "Choose to exile anyone you want";
    public Color RoleColor => TownOfSushiColors.Prosecutor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;

    public bool IsPowerCrew =>
        ProsecutionsCompleted <
        (int)OptionGroupSingleton<ProsecutorOptions>.Instance
            .MaxProsecutions; // Disable end game checks if prosecutes are available

    public CustomRoleConfiguration Configuration => new(this)
    {
        MaxRoleCount = 1,
        Icon = TOSRoleIcons.Prosecutor,
        IntroSound = TOSAudio.ProsIntroSound
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var text = ITownOfSushiRole.SetNewTabText(this);
        if (PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && !allyMod.GetsPunished)
        {
            text.AppendLine(TownOfSushiPlugin.Culture, $"<b>You may prosecute crew.</b>");
        }

        var prosecutes = OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions - ProsecutionsCompleted;
        var newText = prosecutes == 1 ? "1 Prosecution Remaining." : $"\n{prosecutes} Prosecutions Remaining.";
        text.AppendLine(TownOfSushiPlugin.Culture, $"{newText}");
        return text;
    }

    public string GetAdvancedDescription()
    {
        return
            "The Prosecutor is a Crewmate Power role that can Exile a player, applying 5 votes to a player of their choosing. They can also see who voted for who, even if they’re anonymous."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Prosecute (Meeting)",
            "Exile any player of your choosing, throwing 5 votes on them and ignoring all other votes.",
            TOSRoleIcons.Prosecutor)
    ];

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.HasModifier<ImitatorCacheModifier>())
        {
            ProsecutionsCompleted = (int)OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions;
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        var meeting = MeetingHud.Instance;
        if (!Player.AmOwner || meeting == null ||
            ProsecutionsCompleted >= OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions)
        {
            return;
        }

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
        ProsecuteVictim = byte.MaxValue;

        if (HasProsecuted)
        {
            ProsecutionsCompleted++;
        }

        HasProsecuted = false;
    }

    [MethodRpc((uint)TownOfSushiRpc.Prosecute, SendImmediately = true)]
    public static void RpcProsecute(PlayerControl plr, byte Victim)
    {
        if (plr.Data.Role is not ProsecutorRole prosecutorRole)
        {
            return;
        }

        if (prosecutorRole.ProsecutionsCompleted >=
            OptionGroupSingleton<ProsecutorOptions>.Instance.MaxProsecutions)
        {
            return;
        }

        prosecutorRole.HasProsecuted = true;
        prosecutorRole.ProsecuteVictim = Victim;
    }
}