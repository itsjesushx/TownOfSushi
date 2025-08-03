using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfSushi.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class DetectiveRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable
{
    public override bool IsAffectedByComms => false;

    public CrimeSceneComponent? InvestigatingScene { get; set; }

    [HideFromIl2Cpp] public List<byte> InvestigatedPlayers { get; init; } = new();

    public string RoleName => "Detective";
    public string RoleDescription => "Inspect Crime Scenes To Catch The Killer";
    public string RoleLongDescription => "Inspect crime scenes, then examine players to see if they were at the scene.";
    public Color RoleColor => TownOfSushiColors.Detective;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Detective,
        IntroSound = TOSAudio.QuestionSound
    };

    public void LobbyStart()
    {
        InvestigatingScene = null;
        InvestigatedPlayers.Clear();

        CrimeSceneComponent.Clear();
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            "The Detective can inspect a crime scene and examine players to see if they were at the crime scene, flashing red if they were there."
            + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Inspect",
            "Crime scenes will spawn with all dead bodies. Inspect the crime scene and then examine players to discover clues. During the next meeting, you will recieve a report revealing the killer's role.",
            TOSCrewAssets.InspectSprite),
        new("Examine",
            "Examine players after inspecting a crime scene. You will be told if the player was at the crime scene.",
            TOSCrewAssets.ExamineSprite)
    ];

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);

        InvestigatingScene = null;
        InvestigatedPlayers.Clear();
    }

    public void ExaminePlayer(PlayerControl player)
    {
        if (InvestigatedPlayers.Contains(player.PlayerId))
        {
            Coroutines.Start(MiscUtils.CoFlash(Color.red));

            var deadPlayer = InvestigatingScene?.DeadPlayer!;

            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Detective.ToTextColor()}{player.Data.PlayerName} was at the scene of {deadPlayer.Data.PlayerName}'s death!\nThey might be the killer or a witness.</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Detective.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }
        else
        {
            Coroutines.Start(MiscUtils.CoFlash(Color.green));
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Detective.ToTextColor()}{player.Data.PlayerName} was not at the scene of the crime.</b></color>",
                Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Detective.LoadAsset());
            notif1.Text.SetOutlineThickness(0.35f);
        }
    }

    public void Report(byte deadPlayerId)
    {
        var areReportsEnabled = OptionGroupSingleton<DetectiveOptions>.Instance.DetectiveReportOn;

        if (!areReportsEnabled)
        {
            return;
        }

        var matches = GameHistory.KilledPlayers.Where(x => x.VictimId == deadPlayerId).ToArray();

        DeadPlayer? killer = null;

        if (matches.Length > 0)
        {
            killer = matches[0];
        }

        if (killer == null)
        {
            return;
        }

        var br = new BodyReport
        {
            Killer = MiscUtils.PlayerById(killer.KillerId),
            Reporter = Player,
            Body = MiscUtils.PlayerById(killer.VictimId),
            KillAge = (float)(DateTime.UtcNow - killer.KillTime).TotalMilliseconds
        };

        var reportMsg = BodyReport.ParseDetectiveReport(br);

        if (string.IsNullOrWhiteSpace(reportMsg))
        {
            return;
        }

        // Send the message through chat only visible to the detective
        var title = $"<color=#{TownOfSushiColors.Detective.ToHtmlStringRGBA()}>Detective Report</color>";
        var reported = Player;
        if (br.Body != null)
        {
            reported = br.Body;
        }

        MiscUtils.AddFakeChat(reported.Data, title, reportMsg, false, true);
    }
}