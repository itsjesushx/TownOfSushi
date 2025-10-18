using System.Collections;
using System.Text;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Patches.Stubs;
using Reactor.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Patches;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;
using UnityEngine.UI;
// using Reactor.Utilities.Extensions;

namespace TownOfSushi.Roles.Crewmate;

public sealed class HaunterRole(IntPtr cppPtr) : CrewmateGhostRole(cppPtr), ITownOfSushiRole, IGhostRole, IWikiDiscoverable
{
    public bool Revealed => TaskStage is GhostTaskStage.Revealed or GhostTaskStage.CompletedTasks;
    public bool CompletedAllTasks => TaskStage is GhostTaskStage.CompletedTasks;

    public bool Setup { get; set; }
    public bool Caught { get; set; }
    public bool Faded { get; set; }
    public bool CanBeClicked
    {
        get
        {
            return TaskStage is GhostTaskStage.Clickable || TaskStage is GhostTaskStage.Revealed || TaskStage is GhostTaskStage.CompletedTasks;
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
        var options = OptionGroupSingleton<HaunterOptions>.Instance;

        if (options.HaunterCanBeClickedBy == HaunterRoleClickableType.ImpsOnly &&
            !PlayerControl.LocalPlayer.IsImpostor())
        {
            return false;
        }

        if (options.HaunterCanBeClickedBy == HaunterRoleClickableType.NonCrew &&
            !(PlayerControl.LocalPlayer.IsImpostor() || PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling)
                || PlayerControl.LocalPlayer.TryGetModifier<AllianceGameModifier>(out var allyMod) && allyMod.GetsPunished))
        {
            return false;
        }

        return true;
    }

    public void Spawn()
    {
        Setup = true;

        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Error($"Setup HaunterRole '{Player.Data.PlayerName}'");
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

            // Logger<TownOfSushiPlugin>.Message($"HaunterRole.FadeUpdate UnFaded");
        }
    }

    public void Clicked()
    {
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Message($"HaunterRole.Clicked");
        Caught = true;
        Player.Exiled();

        if (Player.AmOwner)
        {
            HudManager.Instance.AbilityButton.SetEnabled();
        }

        Player.RemoveModifier<HaunterArrowModifier>();
    }

    public string RoleName => "Haunter";
    public string RoleDescription => string.Empty;
    public string RoleLongDescription => "Complete all your tasks without getting caught to reveal impostors!";
    public Color RoleColor => TownOfSushiColors.Haunter;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

    public CustomRoleConfiguration Configuration => new(this)
    {
        Icon = TOSRoleIcons.Haunter,
        TasksCountForProgress = false,
        HideSettings = false,
        ShowInFreeplay = true
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return
            $"The {RoleName} is a Crewmate Ghost who can do tasks. They will appear as a transparent player. " +
            "If they finish all their tasks, all alive players will see who the Impostors are. " +
            "However, if an Impostor clicks them first, they will become a normal ghost. " +
            $"Impostors get a warning shortly before and when the {RoleName} finishes their tasks. "
            + MiscUtils.AppendOptionsText(GetType());
    }
    // public DangerMeter ImpostorMeter { get; set; }

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
                // var dangerMeter = GameManagerCreator.Instance.HideAndSeekManagerPrefab.LogicDangerLevel.dangerMeter;
                // ImpostorMeter = UnityEngine.Object.Instantiate(dangerMeter, HudManager.Instance.transform.parent);
            }
        }

        MiscUtils.AdjustGhostTasks(player);
    }

    /* public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not HaunterRole || !Player.AmOwner) return;

        float num = float.MaxValue;
        var dangerLevel1 = 0f;
        var dangerLevel2 = 0f;
        var scaryMusicDistance = 55f * GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod;
        var veryScaryMusicDistance = 15f * GameOptionsManager.Instance.currentNormalGameOptions.PlayerSpeedMod;
        foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsImpostor() || x.IsNeutral()))
        {
            if (player != null)
            {
                float sqrMagnitude = (player.transform.position - Player.transform.position).sqrMagnitude;
                if (sqrMagnitude < scaryMusicDistance && num > sqrMagnitude)
                {
                    num = sqrMagnitude;
                }
            }
        }
        dangerLevel1 = Mathf.Clamp01((scaryMusicDistance - num) / (scaryMusicDistance - veryScaryMusicDistance));
        dangerLevel2 = Mathf.Clamp01((veryScaryMusicDistance - num) / veryScaryMusicDistance);
        ImpostorMeter.SetDangerValue(dangerLevel1, dangerLevel2);
    } */
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
        /* if (Player.AmOwner)
        {
            ImpostorMeter.DestroyImmediate();
        } */
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

        if (TaskStage is GhostTaskStage.Unclickable && tasksRemaining ==
            (int)OptionGroupSingleton<HaunterOptions>.Instance.NumTasksLeftBeforeClickable)
        {
            TaskStage = GhostTaskStage.Clickable;
            if (Player.AmOwner)
            {
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Haunter,
                    $"<b>You are now clickable by players!</b>"), Color.white,
                    new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Haunter.LoadAsset());
                notif1.AdjustNotification();
            }
        }

        if (!Revealed && tasksRemaining == (int)OptionGroupSingleton<HaunterOptions>.Instance.NumTasksLeftBeforeAlerted)
        {
            TaskStage = GhostTaskStage.Revealed;

            if (Player.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(RoleColor));
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Haunter,
                    $"<b>You have alerted the Killers!</b>"), Color.white,
                    new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Haunter.LoadAsset());
                notif1.AdjustNotification();
            }
            else if (IsTargetOfHaunter(PlayerControl.LocalPlayer))
            {
                // Logger<TownOfSushiPlugin>.Error($"CheckTaskRequirements IsTargetOfHaunter");
                Coroutines.Start(MiscUtils.CoFlash(RoleColor));

                Player.AddModifier<HaunterArrowModifier>(PlayerControl.LocalPlayer, RoleColor);
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Haunter,
                    $"<b>A Haunter is loose, catch them before they reveal you!</b>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Haunter.LoadAsset());
                notif1.AdjustNotification();
            }
        }

        if (!CompletedAllTasks && completedTasks == realTasks.Count)
        {
            TaskStage = GhostTaskStage.CompletedTasks;

            if (Player.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(Color.white));
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Haunter,
                    $"<b>You have revealed the Killers!</b>"), Color.white,
                    new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Haunter.LoadAsset());
                notif1.AdjustNotification();
            }
            else if (IsTargetOfHaunter(PlayerControl.LocalPlayer))
            {
                // Logger<TownOfSushiPlugin>.Error($"CheckTaskRequirements IsTargetOfHaunter");
                Coroutines.Start(MiscUtils.CoFlash(Color.white));

                Player.AddModifier<HaunterArrowModifier>(PlayerControl.LocalPlayer, RoleColor);
                var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.Haunter,
                    $"<b>The Haunter has completed their tasks!</b>"),
                    Color.white, new Vector3(0f, 1f, -20f), spr: TOSRoleIcons.Haunter.LoadAsset());
                notif1.AdjustNotification();
            }
        }
        if (TownOfSushiPlugin.IsDevBuild) Logger<TownOfSushiPlugin>.Error($"Haunter Stage for '{Player.Data.PlayerName}': {TaskStage.ToDisplayString()} - ({completedTasks} / {realTasks.Count})");
    }

    public static bool IsTargetOfHaunter(PlayerControl player)
    {
        if (player == null || player.Data == null || player.Data.Role == null)
        {
            return false;
        }

        return player.IsImpostor() || (player.Is(RoleAlignment.NeutralKilling) &&
                                       OptionGroupSingleton<HaunterOptions>.Instance.RevealNeutralRoles);
    }

    public static bool HaunterVisibilityFlag(PlayerControl player)
    {
        var haunter = MiscUtils.GetRole<HaunterRole>();

        if (haunter == null)
        {
            return false;
        }

        return IsTargetOfHaunter(player) && haunter.CompletedAllTasks && !player.AmOwner;
    }
    public enum GhostTaskStage
    {
        Unclickable,
        Clickable,
        Revealed,
        CompletedTasks
    }
}