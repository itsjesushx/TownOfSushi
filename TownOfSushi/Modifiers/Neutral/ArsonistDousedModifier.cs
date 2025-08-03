using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class ArsonistDousedModifier(byte arsonistId) : BaseModifier
{
    public override string ModifierName => "Doused";
    public override bool HideOnUi => true;
    public byte ArsonistId { get; } = arsonistId;

    public override void OnActivate()
    {
        var arso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == ArsonistId);
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.ArsonistDouse, arso!, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<ArsonistRole>())
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(Color.yellow));
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.yellow));
    }
}
