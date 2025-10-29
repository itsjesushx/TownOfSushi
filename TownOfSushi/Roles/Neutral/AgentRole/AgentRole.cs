using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Reactor.Utilities;
using MiraAPI.Patches.Stubs;


namespace TownOfSushi.Roles.Neutral;

public sealed class AgentRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public string RoleName => "Agent";
    public string RoleDescription => "Finish Your Tasks To Get New Abilities";
    public string RoleLongDescription => "Finish your tasks to become the Hitman. \nAnother role with better abilities!";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<BodyguardRole>());
    public Color RoleColor => TownOfSushiColors.Agent;
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public bool HasImpostorVision => OptionGroupSingleton<AgentOptions>.Instance.HasImpostorVision;
    public bool CompletedAllTasks { get; private set; }
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
        IntroSound = TOSAudio.AdministratorIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TOSRoleIcons.Hitman,
        TasksCountForProgress = false,
    };

    public void CheckTaskRequirements()
    {
        var completedTasks = Player.myTasks.ToArray().Count(t => t.IsComplete);

        CompletedAllTasks = completedTasks == Player.myTasks.Count;

        if (CompletedAllTasks && Player.AmOwner)
        {
            Coroutines.Start(MiscUtils.CoFlash(Color.blue, alpha: 0.5f, PlaySound: true));
        }
        Player.RpcChangeRole(RoleId.Get<HitmanRole>());
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Agent);
        }
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<AgentRole>().Count(x => !x.Player.HasDied());

        if (MiscUtils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Agent is a Neutral Killing role that has to finish their tasks in order to become a Hitman. Their tasks do not count for a task win." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("None",
            "Finish tasks to become a Hitman. You may be able to vent depending on settings.",
            MiraAssets.Empty)    
    ];
}