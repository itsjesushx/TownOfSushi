using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class HitmanDragDropButton : TownOfSushiRoleButton<HitmanRole, DeadBody>, IAftermathableBodyButton
{
    public override string Name => "Drag";
    public override string Keybind => "tos.ActionCustom";
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<AgentOptions>.Instance.DragCooldown + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.DragSprite;

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

        if (PlayerControl.LocalPlayer.HasModifier<HitmanDragModifier>())
        {
            HitmanRole.RpcStopDragging(PlayerControl.LocalPlayer, Target.transform.position);
            Timer = Cooldown;
        }
        else
        {
            HitmanRole.RpcStartDragging(PlayerControl.LocalPlayer, Target.ParentId);
        }
    }

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

    public override bool CanUse()
    {
        return base.CanUse() && Target && !PlayerControl.LocalPlayer.inVent && (!PlayerControl.LocalPlayer.HasModifier<HitmanDragModifier>() || CanDrop());
    }

    private bool CanDrop()
    {
        if (Target == null)
        {
            return false;
        }

        return !PhysicsHelpers.AnythingBetween(PlayerControl.LocalPlayer.Collider, PlayerControl.LocalPlayer.Collider.bounds.center, Target.TruePosition, Constants.ShipAndAllObjectsMask, false);
    }

    public void SetDrag()
    {
        OverrideSprite(TOSNeutAssets.DragSprite.LoadAsset());
        OverrideName("Drag");
    }

    public void SetDrop()
    {
        OverrideSprite(TOSNeutAssets.DropSprite.LoadAsset());
        OverrideName("Drop");
    }

    public override bool IsTargetValid(DeadBody? target)
    {
        return target && target?.Reported == false;
    }
}