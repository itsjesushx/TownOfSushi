using System.Collections;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Patches;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using UnityEngine.UI;

namespace TownOfSushi.Roles.Neutral;

public sealed class SpectreRole(IntPtr cppPtr)
    : NeutralGhostRole(cppPtr), ITownOfSushiRole, IGhostRole, IWikiDiscoverable
{
    public bool CompletedAllTasks => TaskStage is GhostTaskStage.CompletedTasks;

    public bool Setup { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }

    public bool CanBeClicked
    {
        get
        {
            return TaskStage is GhostTaskStage.Clickable or GhostTaskStage.Revealed;
        }
        set
        {
            // Left Alone
        }
    }
    public GhostTaskStage TaskStage { get; private set; } = GhostTaskStage.Unclickable;
    public bool GhostActive => Setup && !Caught;

    public bool CanCatch()
    {
        return true;
    }

    public void Spawn()
    {
        Setup = true;

        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Error($"Setup SpectreRole '{Player.Data.PlayerName}'");
        Player.gameObject.layer = LayerMask.NameToLayer("Players");

        Player.gameObject.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        Player.gameObject.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => Player.OnClick()));
        Player.gameObject.GetComponent<BoxCollider2D>().enabled = true;

        if (Player.AmOwner)
        {
            Player.SpawnAtRandomVent();
            Player.MyPhysics.ResetMoveState();

            HudManager.Instance.SetHudActive(false);
            HudManager.Instance.SetHudActive(true);
            HudManager.Instance.AbilityButton.SetDisabled();
            HudManagerPatches.ResetZoom();
        }
    }

    public void FadeUpdate(HudManager instance)
    {
        if (!Caught && Setup)
        {
            Player.GhostFade();
            Faded = true;
        }
        else if (Faded)
        {
            Player.ResetAppearance();
            Player.cosmetics.ToggleNameVisible(true);

            Player.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
            Player.MyPhysics.ResetMoveState();

            Faded = false;

            // if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Message($"SpectreRole.FadeUpdate UnFaded");
        }
    }

    public void Clicked()
    {
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Message($"SpectreRole.Clicked");
        Caught = true;
        Player.Exiled();

        if (Player.AmOwner)
        {
            HudManager.Instance.AbilityButton.SetEnabled();
        }
    }

    public override string RoleName => "Spectre";
    public override string RoleDescription => string.Empty;
    public override string RoleLongDescription => "Complete all your tasks without being caught!";
    public override Color RoleColor => TownOfSushiColors.Spectre;
    public override RoleAlignment RoleAlignment => RoleAlignment.NeutralEvil;

    public override CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Spectre,
        HideSettings = false,
        ShowInFreeplay = true
    };

    public bool MetWinCon => CompletedAllTasks;

    public override bool WinConditionMet()
    {
        return OptionGroupSingleton<SpectreOptions>.Instance.SpectreWin is SpectreWinOptions.EndsGame &&
               CompletedAllTasks;
    }

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Neutral Ghost role that wins the game by finishing their tasks before a alive player has clicked on them." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public override void UseAbility()
    {
        if (GhostActive)
        {
            return;
        }

        base.UseAbility();
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (TutorialManager.InstanceExists)
        {
            Setup = true;

            Coroutines.Start(SetTutorialCollider(Player));

            if (Player.AmOwner)
            {
                Player.MyPhysics.ResetMoveState();

                HudManager.Instance.SetHudActive(false);
                HudManager.Instance.SetHudActive(true);
                HudManager.Instance.AbilityButton.SetDisabled();
                HudManagerPatches.ResetZoom();
            }
        }

        MiscUtils.AdjustGhostTasks(player);
    }

    private static IEnumerator SetTutorialCollider(PlayerControl player)
    {
        yield return new WaitForSeconds(0.01f);

        player.gameObject.layer = LayerMask.NameToLayer("Players");

        player.gameObject.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
        player.gameObject.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => player.OnClick()));
        player.gameObject.GetComponent<BoxCollider2D>().enabled = true;
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (TutorialManager.InstanceExists)
        {
            Player.ResetAppearance();
            Player.cosmetics.ToggleNameVisible(true);

            Player.cosmetics.currentBodySprite.BodySprite.color = Color.white;
            Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
            Player.MyPhysics.ResetMoveState();

            Faded = false;
        }
        else if (!Player.HasModifier<BasicGhostModifier>())
        {
            Player.AddModifier<BasicGhostModifier>();
        }
    }

    public override bool CanUse(IUsable console)
    {
        var validUsable = console.TryCast<Console>() ||
                          console.TryCast<DoorConsole>() ||
                          console.TryCast<OpenDoorConsole>() ||
                          console.TryCast<DeconControl>() ||
                          console.TryCast<PlatformConsole>() ||
                          console.TryCast<Ladder>() ||
                          console.TryCast<ZiplineConsole>();

        return GhostActive && validUsable;
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return CompletedAllTasks;
    }

    public void CheckTaskRequirements()
    {
        if (Caught)
        {
            return;
        }
        var realTasks = Player.myTasks.ToArray()
            .Where(x => !PlayerTask.TaskIsEmergency(x) && !x.TryCast<ImportantTextTask>()).ToList();
        
        var completedTasks = realTasks.Count(t => t.IsComplete);
        var tasksRemaining = realTasks.Count - completedTasks;

        if (TaskStage is GhostTaskStage.Unclickable && tasksRemaining <=
            (int)OptionGroupSingleton<SpectreOptions>.Instance.NumTasksLeftBeforeClickable)
        {
            TaskStage = GhostTaskStage.Clickable;
            if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Spectre, $"<b>You are now clickable by players!</b>"), Color.white,
                    new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Spectre.LoadAsset());
                notif1.AdjustNotification();
            }
        }

        if (completedTasks == realTasks.Count)
        {
            TaskStage = GhostTaskStage.CompletedTasks;
        }
        
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Error($"Spectre Stage for '{Player.Data.PlayerName}': {TaskStage.ToDisplayString()} - ({completedTasks} / {realTasks.Count})");

        if (OptionGroupSingleton<SpectreOptions>.Instance.SpectreWin is not SpectreWinOptions.Spooks ||
            !CompletedAllTasks)
        {
            return;
        }

        if (!Player.AmOwner)
        {
            return;
        }

        var allVictims = PlayerControl.AllPlayerControls.ToArray()
            .Where(x => !x.AmOwner);
                
        if (!allVictims.Any())
        {
            return;
        }

        foreach (var player in allVictims)
        {
            player.AddModifier<MisfortuneTargetModifier>();
        }

        var spookButton = CustomButtonSingleton<SpectreSpookButton>.Instance;
        spookButton.Show = true;
        spookButton.SetActive(true, this);
    }
}

public enum GhostTaskStage
{
    Unclickable,
    Clickable,
    Revealed,
    CompletedTasks
}