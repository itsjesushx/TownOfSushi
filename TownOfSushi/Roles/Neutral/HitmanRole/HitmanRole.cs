using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using MiraAPI.Patches.Stubs;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class HitmanRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public string RoleName => "Hitman";
    public string RoleDescription => "Murder, morph and drag bodies";
    public string RoleLongDescription => "You are able to drag dead bodies, \nMorph into other players and kill everyone!";
    public MysticClueType MysticHintType => MysticClueType.Trickster;
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public Color RoleColor => TownOfSushiColors.Hitman;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
        IntroSound = TOSAudio.PredatorTerminateSound,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        HideSettings = true,
        CanModifyChance = false,
        MaxRoleCount = 0,
        Icon = TOSRoleIcons.Hitman,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Hitman);
        }
    }
    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TOSAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Impostor);
        }
    }

    public bool HasImpostorVision => true;

    public override bool DidWin(GameOverReason gameOverReason)
    {
        return WinConditionMet();
    }

    public override bool CanUse(IUsable usable)
    {
        if (!GameManager.Instance.LogicUsables.CanUse(usable, Player))
        {
            return false;
        }
        Console console = usable.TryCast<Console>()!;
        return (console == null) || console.AllowImpostor;
    }

    public bool WinConditionMet()
    {
        var roleCount = CustomRoleUtils.GetActiveRolesOfType<HitmanRole>().Count(x => !x.Player.HasDied());

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

    [MethodRpc((uint)TownOfSushiRpc.HitmanDragBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStartDragging(PlayerControl playerControl, byte bodyId)
    {
        playerControl.GetModifierComponent()?.AddModifier(new HitmanDragModifier(bodyId));

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HitmanDrag, playerControl, Helpers.GetBodyById(bodyId));
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (playerControl.AmOwner)
        {
            CustomButtonSingleton<HitmanDragDropButton>.Instance.SetDrop();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.HitmanDropBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStopDragging(PlayerControl playerControl, Vector2 dropLocation)
    {
        var dragMod = playerControl.GetModifier<HitmanDragModifier>()!;
        var dropPos = (Vector3)dropLocation;
        dropPos.z = dropPos.y / 1000f;
        dragMod.DeadBody!.transform.position = dropPos;

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HitmanDrop, playerControl, dragMod.DeadBody);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        playerControl.GetModifierComponent()?.RemoveModifier(dragMod);

        if (playerControl.AmOwner)
        {
            CustomButtonSingleton<HitmanDragDropButton>.Instance.SetDrag();
        }
    }

    public string GetAdvancedDescription()
    {
        return "The Hitman is a Neutral role with its own win condition. The Hitman's aim is to kill win alone. The Hitman is able to kill players, morph into them like a Morphling or a Glitch for a set amount of time. They can also drag dead bodies just like an Hitman. They may be able to vent depending on settings." + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Morph",
            "Copy the appearance of another player, taking on their whole look.",
            TOSImpAssets.MorphSprite),
        new("Drag",
            "Drag dead bodies around the map.",
            TOSImpAssets.DragSprite) 
    ];
}