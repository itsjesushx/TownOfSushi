using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class WitchSpelledModifier(byte witchId) : BaseModifier
{
    public override string ModifierName => "Spelled";
    public override bool HideOnUi => true;

    public byte WitchId { get; } = witchId;

    private Color color = new(0.9f, 1f, 0.7f, 1f);

    public override void OnActivate()
    {
        base.OnActivate();

        var pb = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == WitchId);
        var TosAbilityEvent = new TOSAbilityEvent(AbilityType.WitchCurse, pb!, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<PlaguebearerRole>() && Player != PlayerControl.LocalPlayer)
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(color));
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(color));
    }
}