using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class PoliticianRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITOSCrewRole, IWikiDiscoverable, IMysticClue
{
    private MeetingMenu meetingMenu;
    public override bool IsAffectedByComms => false;
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public bool CanCampaign { get; set; } = true;
    public string RoleName => "Politician";
    public string RoleDescription => "Campaign To Become The Mayor!";
    public string RoleLongDescription => "Spread your campaign to become the Mayor!";
    public Color RoleColor => TownOfSushiColors.Politician;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;
    public bool IsPowerCrew => true;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Politician,
        IntroSound = TOSAudio.MayorRevealSound,
        MaxRoleCount = 1
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        if (PlayerControl.LocalPlayer.HasModifier<EgotistModifier>())
        {
            stringB.AppendLine(CultureInfo.InvariantCulture,
                $"<b>The Impostors will know your true motives when revealed.</b>");
        }

        return stringB;
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Power role that can reveal themselves to the crew as the Mayor, given that they have campaigned at least half of the crewmates."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Campaign",
            "Give a player a ballot, which will only be useful to you if they are a Crewmate.",
            TOSCrewAssets.CampaignButtonSprite),
        new("Reveal (Meeting)",
            "If you reveal and you have more than half of the crewmates campaigned (or no other crewmates remain), you will become the Mayor! Otherwise, your ability will fail" +
            (OptionGroupSingleton<PoliticianOptions>.Instance.PreventCampaign ? " and you won't be able to campaign the following round." : "."),
            TOSAssets.RevealCleanSprite)
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
                TOSAssets.RevealButtonSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.35f, 0f, -3f)
            };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        CanCampaign = true;

        if (Player.AmOwner)
            // Logger<TownOfSushiPlugin>.Message($"PoliticianRole.OnMeetingStart '{Player.Data.PlayerName}' {Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>()}");
        {
            meetingMenu.GenButtons(MeetingHud.Instance,
                Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
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

        if (Player.AmOwner)
        {
            meetingMenu?.Dispose();
            meetingMenu = null!;
        }
    }

    public void Click(PlayerVoteArea voteArea, MeetingHud __)
    {
        if (!Player.AmOwner)
        {
            return;
        }

        meetingMenu.HideButtons();

        var aliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.IsCrewmate());
        var aliveCampaigned = aliveCrew.Count(x => x.HasModifier<PoliticianCampaignedModifier>());
        var hasMajority =
            aliveCampaigned >=
            Math.Max((aliveCrew.Count() - 1) / 2,
                1); // minus one to account for politician, max of at least 1 crewmate campaigned
        if (aliveCrew.Count() == 1)
        {
            hasMajority = true; // if all crew are dead, politician can reveal
        }

        if (hasMajority)
        {
            Player.RpcChangeRole(RoleId.Get<MayorRole>());
        }
        else
        {
            var text = "You need to campaign more Crewmates! You may not reveal again in this meeting.";
            if (OptionGroupSingleton<PoliticianOptions>.Instance.PreventCampaign)
            {
                CanCampaign = false;
                text = "You need to campaign more Crewmates! However, you may not campaign next round.";
            }

            var title = $"<color=#{TownOfSushiColors.Mayor.ToHtmlStringRGBA()}>{RoleName} Feedback</color>";
            MiscUtils.AddFakeChat(Player.Data, title, text, false, true);
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId;
    }
}