using MiraAPI.Events;
using MiraAPI.Hud;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class DragModifier(byte bodyId) : BaseModifier
{
    public override string ModifierName => "Drag";
    public override bool HideOnUi => true;

    public byte BodyId { get; } = bodyId;
    public DeadBody? DeadBody { get; } = Helpers.GetBodyById(bodyId);

    public override bool? CanVent()
    {
        return OptionGroupSingleton<UndertakerOptions>.Instance.CanVentWithBody.Value;
    }
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<UndertakerDragDropButton>.Instance.SetDrag();
        }
        if (DeadBody == null)
        {
            return;
        }
        var dropPos = DeadBody.transform.position;
        dropPos.z = dropPos.y / 1000f;
        DeadBody.transform.position = dropPos;

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.UndertakerDrop, Player, DeadBody);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
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