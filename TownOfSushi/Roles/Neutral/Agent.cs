using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Roles;

using TownOfSushi.Utilities;
using UnityEngine;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Modules.Wiki;
using MiraAPI.Utilities.Assets;
using MiraAPI.Utilities;
using TownOfSushi.Options.Roles.Neutral;
using Reactor.Utilities;

namespace TownOfSushi.Roles.Neutral;

public sealed class AgentRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Agent";
    public string RoleDescription => "Finish Your Tasks To Get New Abilities";
    public string RoleLongDescription => "Finish your tasks to become the Hitman. \nAnother role with better abilities!";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<SnitchRole>());
    public Color RoleColor => Color.blue;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public bool HasImpostorVision => OptionGroupSingleton<AgentOptions>.Instance.HasImpostorVision;
    public bool CompletedAllTasks { get; private set; }
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
        IntroSound = TosAudio.SpyIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = MiraAssets.Empty,
        TasksCountForProgress = false
    };

    public void CheckTaskRequirements()
    {
        var completedTasks = Player.myTasks.ToArray().Count(t => t.IsComplete);

        CompletedAllTasks = completedTasks == Player.myTasks.Count;

        if (CompletedAllTasks && Player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(Color.blue, alpha: 0.5f, PlaySound: true));
            Player.ChangeRole(RoleId.Get<HitmanRole>());
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public bool WinConditionMet()
    {
        if (Player.HasDied()) return false;

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount == 1;

        return result;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Agent is a Neutral Benign role that has to finish their tasks in order to become a new role. Their tasks do not count for a task win." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("None",
            "Finish tasks to gain a new role. You may be able to vent depending on settings.",
            MiraAssets.Empty)    
    ];
}