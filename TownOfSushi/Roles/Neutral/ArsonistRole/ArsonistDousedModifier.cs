using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistDousedModifier(byte ArsonistId) : BaseModifier
{
    public override string ModifierName => "Doused";
    public override bool HideOnUi => true;
    public byte ArsonistId { get; } = ArsonistId;
    public override void OnActivate()
    {
        var arso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == ArsonistId);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.ArsonistDouse, arso!, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<ArsonistRole>())
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(Color.yellow));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.yellow));
    }
}