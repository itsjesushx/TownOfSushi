using System.Text;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Events;
using MiraAPI.Hud;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class UndertakerRole(IntPtr cppPtr)
    : ImpostorRole(cppPtr), ITownOfSushiRole, IWikiDiscoverable, ICrewVariant, IMysticClue
{
    public void FixedUpdate()
    {
        if (Player == null || Player.Data.Role is not UndertakerRole || Player.HasDied() || !Player.AmOwner ||
            MeetingHud.Instance || (!HudManager.Instance.UseButton.isActiveAndEnabled &&
                                    !HudManager.Instance.PetButton.isActiveAndEnabled))
        {
            return;
        }

        HudManager.Instance.KillButton.ToggleVisible(OptionGroupSingleton<UndertakerOptions>.Instance.UndertakerKill ||
                                                     (Player != null && Player.GetModifiers<BaseModifier>()
                                                         .Any(x => x is ICachedRole)) ||
                                                     (Player != null && MiscUtils.ImpAliveCount == 1));
    }

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<RetributionistRole>());
    public string RoleName => "Undertaker";
    public string RoleDescription => "Drag bodies and hide them";
    public string RoleLongDescription => "Drag bodies around to hide them from being reported";
    public MysticClueType MysticHintType => MysticClueType.Death;
    public Color RoleColor => TownOfSushiColors.Impostor;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorSupport;

    public CustomRoleConfiguration Configuration => new(this)
    {
        UseVanillaKillButton = true,
        CanUseVent = OptionGroupSingleton<UndertakerOptions>.Instance.CanVent,
        Icon = TOSRoleIcons.Undertaker
    };

    [HideFromIl2Cpp]
    public StringBuilder SetTabText()
    {
        return ITownOfSushiRole.SetNewTabText(this);
    }

    public string GetAdvancedDescription()
    {
        return "The Undertaker is an Impostor Support role that can drag dead bodies around the map." +
               MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Drag",
            "Drag a dead body, if allowed through settings you can also take it into a vent.",
            TOSImpAssets.DragSprite),
        new("Drop",
            "Drop the dragged dead body, stopping it from being dragged any further.",
            TOSImpAssets.DropSprite)
    ];

    [MethodRpc((uint)TownOfSushiRpc.DragBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStartDragging(PlayerControl playerControl, byte bodyId)
    {
        playerControl.GetModifierComponent()?.AddModifier(new DragModifier(bodyId));

        var TOSAbilityEvent =
            new TOSAbilityEvent(AbilityType.UndertakerDrag, playerControl, Helpers.GetBodyById(bodyId));
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (playerControl.AmOwner)
        {
            CustomButtonSingleton<UndertakerDragDropButton>.Instance.SetDrop();
        }
    }

    [MethodRpc((uint)TownOfSushiRpc.DropBody, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcStopDragging(PlayerControl playerControl, Vector2 dropLocation)
    {
        var dragMod = playerControl.GetModifier<DragModifier>()!;
        var dropPos = (Vector3)dropLocation;
        dropPos.z = dropPos.y / 1000f;
        dragMod.DeadBody!.transform.position = dropPos;

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.UndertakerDrop, playerControl, dragMod.DeadBody);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        playerControl.GetModifierComponent()?.RemoveModifier(dragMod);

        if (playerControl.AmOwner)
        {
            CustomButtonSingleton<UndertakerDragDropButton>.Instance.SetDrag();
        }
    }
}