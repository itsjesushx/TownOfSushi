using MiraAPI.Events;
using MiraAPI.Hud;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class HitmanDragModifier(byte bodyId) : BaseModifier
{
    public override string ModifierName => "Hitman Drag";
    public override bool HideOnUi => true;

    public byte BodyId { get; } = bodyId;
    public DeadBody? DeadBody { get; } = Helpers.GetBodyById(bodyId);
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
        if (Player.AmOwner)
        {
            CustomButtonSingleton<HitmanDragDropButton>.Instance.SetDrag();
        }
        if (DeadBody == null)
        {
            return;
        }
        var dropPos = DeadBody.transform.position;
        dropPos.z = dropPos.y / 1000f;
        DeadBody.transform.position = dropPos;

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HitmanDrop, Player, DeadBody);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override bool? CanVent()
    {
        return OptionGroupSingleton<AgentOptions>.Instance.CanVentWithBody.Value && OptionGroupSingleton<AgentOptions>.Instance.CanUseVents;
    }

    public override void OnActivate()
    {
        if (Player != null)
        {
            Player.MyPhysics.Speed *= OptionGroupSingleton<AgentOptions>.Instance.DragSpeedMultiplier;
            if (OptionGroupSingleton<AgentOptions>.Instance.AffectedSpeed)
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
            Player.MyPhysics.Speed /= OptionGroupSingleton<AgentOptions>.Instance.DragSpeedMultiplier;
            if (OptionGroupSingleton<AgentOptions>.Instance.AffectedSpeed)
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
