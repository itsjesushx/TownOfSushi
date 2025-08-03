using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class UndertakerDragModifier(byte bodyId) : BaseModifier
{
    public override string ModifierName => "Drag";
    public override bool HideOnUi => true;

    public byte BodyId { get; } = bodyId;
    public DeadBody? DeadBody { get; } = Helpers.GetBodyById(bodyId);

    public override bool? CanVent()
    {
        return OptionGroupSingleton<UndertakerOptions>.Instance.CanVentWithBody.Value;
    }

    public override void OnActivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed *= OptionGroupSingleton<UndertakerOptions>.Instance.DragSpeedMultiplier;
            if (OptionGroupSingleton<UndertakerOptions>.Instance.AffectedSpeed)
            {
                var dragged = MiscUtils.PlayerById(DeadBody!.ParentId)!;
                if (dragged.HasModifier<GiantModifier>())
                {
                    Player.MyPhysics.Speed *= OptionGroupSingleton<GiantOptions>.Instance.GiantSpeed;
                }
                else if (dragged.HasModifier<MiniModifier>())
                {
                    Player.MyPhysics.Speed *= OptionGroupSingleton<MiniOptions>.Instance.MiniSpeed;
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed /= OptionGroupSingleton<UndertakerOptions>.Instance.DragSpeedMultiplier;
            if (OptionGroupSingleton<UndertakerOptions>.Instance.AffectedSpeed)
            {
                var dragged = MiscUtils.PlayerById(DeadBody!.ParentId)!;
                if (dragged.HasModifier<GiantModifier>())
                {
                    Player.MyPhysics.Speed /= OptionGroupSingleton<GiantOptions>.Instance.GiantSpeed;
                }
                else if (dragged.HasModifier<MiniModifier>())
                {
                    Player.MyPhysics.Speed /= OptionGroupSingleton<MiniOptions>.Instance.MiniSpeed;
                }
            }
        }
    }

    public override void Update()
    {
        if (DeadBody == null)
        {
            return;
        }

        var targetPos = Player.transform.position;
        targetPos.z = targetPos.y / 1000f;

        DeadBody.transform.position = Vector3.Lerp(DeadBody.transform.position, targetPos, 5f * Time.deltaTime);
    }
}
