using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class PlaguebearerInfectedModifier(byte plaguebearerId) : BaseModifier
{
    public override string ModifierName => "Infected";
    public override bool HideOnUi => true;

    public byte PlagueBearerId { get; } = plaguebearerId;

    private Color color = new(0.9f, 1f, 0.7f, 1f);

    public override void OnActivate()
    {
        base.OnActivate();

        var pb = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == PlagueBearerId);
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.PlaguebearerInfect, pb!, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<PlaguebearerRole>() && Player != PlayerControl.LocalPlayer)
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(color));
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(color));
    }
}
