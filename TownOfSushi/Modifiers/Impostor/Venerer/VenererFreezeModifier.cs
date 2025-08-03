using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor.Venerer;

public sealed class VenererFreezeModifier(PlayerControl venerer) : TimedModifier, IVenererModifier
{
    public override string ModifierName => "Freeze";
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<VenererOptions>.Instance.AbilityDuration;

    public PlayerControl Venerer { get; set; } = venerer;
    public float SpeedFactor { get; set; }

    public override void OnActivate()
    {
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.VenererFreezeAbility, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Player.HasDied() || Venerer.HasDied()) return;

        var minFreezeSpeed = OptionGroupSingleton<VenererOptions>.Instance.MinFreezeSpeed;
        var freezeRadius = OptionGroupSingleton<VenererOptions>.Instance.FreezeRadius * ShipStatus.Instance.MaxLightRadius;
        var rangeFromVenerer = (Player.GetTruePosition() - Venerer.GetTruePosition()).magnitude;

        SpeedFactor = 1f;

        if (rangeFromVenerer < freezeRadius)
        {
            SpeedFactor = Mathf.Lerp(minFreezeSpeed, 1f, rangeFromVenerer / freezeRadius);
        }
    }
}
