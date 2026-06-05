using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using Reactor.Utilities;
using MiraAPI.Patches.Stubs;

namespace TownOfSushi.Roles.Neutral;

public sealed class AgentRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITOSRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public string RoleName => "Agent";
    public string RoleDescription => "Finish your tasks to get new abilities";
    public string RoleLongDescription => "Finish your tasks to become the Hitman. \nAnother role with better abilities!";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<BodyguardRole>());
    public Color RoleColor => TownOfSushiColors.Agent;
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Factions Faction => Factions.Neutral;
    public bool HasImpostorVision => OptionGroupSingleton<AgentOptions>.Instance.HasImpostorVision;
    public bool CompletedAllTasks { get; private set; }
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
        IntroSound = TownOfSushiAudio.AdministratorIntroSound,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
        Icon = TownOfSushiAssets.Hitman,
        TasksCountForProgress = false,
    };

    public void CheckTaskRequirements()
    {
        var realTasks = Player.myTasks.ToArray().Where(x => !PlayerTask.TaskIsEmergency(x) && !x.TryCast<ImportantTextTask>()).ToList();
        if (realTasks.Count <= 1)
        {
            return;
        }

        var completedTasks = realTasks.Count(t => t.IsComplete);
        if (completedTasks == realTasks.Count)
        {
            CompletedAllTasks = true;
        }

        if (CompletedAllTasks && Player.AmOwner)
        {
            Coroutines.Start(Utils.CoFlash(Color.blue, alpha: 0.5f, PlaySound: true));
        }
        Player.RpcChangeRole(RoleId.Get<HitmanRole>());
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TownOfSushiAssets.VentSprite.LoadAsset();
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

        if (Utils.KillersAliveCount > roleCount)
        {
            return false;
        }

        return roleCount >= Helpers.GetAlivePlayers().Count - roleCount;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITOSRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Agent is a Neutral Killing role that has to finish their tasks in order to become a Hitman. Their tasks do not count for a task win." + Utils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = 
    [
        new("None",
            "Finish tasks to become a Hitman. You may be able to vent depending on settings.",
            MiraAssets.Empty)    
    ];
}