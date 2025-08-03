using System.Globalization;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Modifiers.Game.Alliance;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;

using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class PoliticianRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITouCrewRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Politician";
    public string RoleDescription => "Campaign To Become The Mayor!";
    public string RoleLongDescription => "Spread your campaign to become the Mayor!";
    public Color RoleColor => TownOfSushiColors.Politician;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmatePower;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public override bool IsAffectedByComms => false;
    public bool IsPowerCrew => true;
    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TosRoleIcons.Politician,
        IntroSound = TosAudio.MayorRevealSound,
        MaxRoleCount = 1,
    };

    public bool CanCampaign { get; set; } = true;

    private MeetingMenu meetingMenu;

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);

        if (Player.AmOwner)
        {
            meetingMenu = new MeetingMenu(
                this,
                Click,
                MeetingAbilityType.Click,
                TosAssets.RevealButtonSprite,
                null!,
                IsExempt)
            {
                Position = new Vector3(-0.35f, 0f, -3f),
            };
        }
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        CanCampaign = true;

        if (Player.AmOwner)
        {
            // Logger<TownOfSushiPlugin>.Message($"PoliticianRole.OnMeetingStart '{Player.Data.PlayerName}' {Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>()}");
            meetingMenu.GenButtons(MeetingHud.Instance, Player.AmOwner && !Player.HasDied() && !Player.HasModifier<JailedModifier>());
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
        if (!Player.AmOwner) return;

        meetingMenu.HideButtons();

        var aliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.HasDied() && x.IsCrewmate());
        var aliveCampaigned = aliveCrew.Count(x => x.HasModifier<PoliticianCampaignedModifier>());
        var hasMajority = aliveCampaigned >= Math.Max(aliveCrew.Count() / 2 - 1, 1); // minus one to account for politician, max of at least 1 crewmate campaigned
        if (!aliveCrew.Any(x => x.Data.Role is not PoliticianRole)) hasMajority = true; // if all crew are dead, politician can reveal

        if (hasMajority)
        {
            Player.RpcChangeRole(RoleId.Get<MayorRole>());
            if (Player.HasModifier<ToBecomeTraitorModifier>())
            {
                Player.GetModifier<ToBecomeTraitorModifier>()!.Clear();
            }
        }
        else
        {
            var text = "You need to campaign more Crewmates! You may not reveal again in this meeting.";
            if (OptionGroupSingleton<PoliticianOptions>.Instance.PreventCampaign)
            {
                CanCampaign = false;
                text = "You need to campaign more Crewmates! However, you may not campaign next round.";
            }
            var title = $"<color=#{TownOfSushiColors.Mayor.ToHtmlStringRGBA()}>Politician Feedback</color>";
            MiscUtils.AddFakeChat(Player.Data, title, text, false, true);
        }
    }

    public bool IsExempt(PlayerVoteArea voteArea)
    {
        return voteArea?.TargetPlayerId != Player.PlayerId;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        var stringB = ITownOfSushiRole.SetNewTabText(this);
        if (PlayerControl.LocalPlayer.HasModifier<EgotistModifier>())
        {
            stringB.AppendLine(CultureInfo.InvariantCulture, $"<b>The Impostors will know your true motives when revealed.</b>");
        }

        return stringB;
    }
    public string GetAdvancedDescription()
    {
        return "The Politician is a Crewmate Power role that can reveal themselves to the crew as the Mayor, given that they have campaigned at least half of the crewmates."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Campaign",
            $"Give a player a ballot, which will only be useful to you if they are a Crewmate.",
            TosCrewAssets.CampaignButtonSprite),
        new("Reveal (Meeting)",
            $"If you reveal and you have more than half of the crewmates campaigned (or no other crewmates remain), you will become the Mayor! Otherwise, your ability will fail and you " + (OptionGroupSingleton<PoliticianOptions>.Instance.PreventCampaign ? "cannot" : "can") + " campaign the following round.",
            TosAssets.RevealCleanSprite),
    ];
}
