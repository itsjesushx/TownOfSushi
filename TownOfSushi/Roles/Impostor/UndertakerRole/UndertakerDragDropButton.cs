using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class UndertakerDragDropButton : TownOfSushiRoleButton<UndertakerRole, DeadBody>, IAftermathableBodyButton
{
    public override string Name => "Drag";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<UndertakerOptions>.Instance.DragCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.DragSprite;

    public override void ClickHandler()
    {
        if (!CanClick())
        {
            return;
        }

        if (LimitedUses)
        {
            UsesLeft--;
            Button?.SetUsesRemaining(UsesLeft);
        }

        OnClick();
        Button?.SetDisabled();
    }

    public override DeadBody? GetTarget()
    {
        return PlayerControl.LocalPlayer?.GetNearestDeadBody(PlayerControl.LocalPlayer.MaxReportDistance / 4f);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.HasModifier<DragModifier>())
        {
            UndertakerRole.RpcStopDragging(PlayerControl.LocalPlayer, Target.transform.position);
            Timer = Cooldown;
        }
        else
        {
            UndertakerRole.RpcStartDragging(PlayerControl.LocalPlayer, Target.ParentId);
        }
    }

    public override bool CanUse()
    {
        return base.CanUse() && Target && !PlayerControl.LocalPlayer.inVent &&
               (!PlayerControl.LocalPlayer.HasModifier<DragModifier>() || CanDrop());
    }

    private bool CanDrop()
    {
        if (Target == null)
        {
            return false;
        }

        return !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider,
            PlayerControl.LocalPlayer.Collider.bounds.center, Target.TruePosition, Constants.ShipAndAllObjectsMask,
            false);
    }

    public void SetDrag()
    {
        OverrideSprite(TOSImpAssets.DragSprite.LoadAsset());
        OverrideName("Drag");
    }

    public void SetDrop()
    {
        OverrideSprite(TOSImpAssets.DropSprite.LoadAsset());
        OverrideName("Drop");
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false;
    }
}