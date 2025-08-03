using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class HitmanRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, IDoomable, ICrewVariant
{
    public string RoleName => "Hitman";
    public string RoleDescription => "Murder, morph into others and drag bodies.";
    public string RoleLongDescription => "You are able to drag dead bodies, \nMorph into other players and kill everyone!";
    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<VeteranRole>());
    public Color RoleColor => TownOfSushiColors.Hitman;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public CustomRoleConfiguration Configuration => new(this)
    {
        CanUseVent = OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
        IntroSound = TosAudio.WerewolfRampageSound,
        DefaultChance = 0,
        DefaultRoleCount = 0,
        HideSettings = true,
        CanModifyChance = false,
        MaxRoleCount = 0,
        Icon = MiraAssets.Empty,
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>(),
    };
    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
        if (Player.AmOwner)
        {
            HudManager.Instance.ImpostorVentButton.graphic.sprite = TosAssets.VentSprite.LoadAsset();
            HudManager.Instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(TownOfSushiColors.Hitman);
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
        if (Player.HasDied()) return false;

        var result = Helpers.GetAlivePlayers().Count <= 2 && MiscUtils.KillersAliveCount == 1;

        return result;
    }
    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    [MethodRpc((uint)TownOfSushiRpc.DragBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStartDragging(PlayerControl playerControl, byte bodyId)
    {
        playerControl.GetModifierComponent()?.AddModifier(new HitmanDragModifier(bodyId));

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.HitmanDrag, playerControl, Helpers.GetBodyById(bodyId));
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        if (playerControl.AmOwner)
        {
            CustomButtonSingleton<HitmanDragDropButton>.Instance.SetDrop();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.DropBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStopDragging(PlayerControl playerControl, Vector2 dropLocation)
    {
        var dragMod = playerControl.GetModifier<HitmanDragModifier>()!;
        var dropPos = (Vector3)dropLocation;
        dropPos.z = dropPos.y / 1000f;
        dragMod.DeadBody!.transform.position = dropPos;

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.HitmanDrop, playerControl, dragMod.DeadBody);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

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
            TosImpAssets.MorphSprite),
        new("Drag",
            "Drag dead bodies around the map.",
            TosImpAssets.DragSprite) 
    ];
}