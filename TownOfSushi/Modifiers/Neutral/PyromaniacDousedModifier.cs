using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class PyromaniacDousedModifier(byte pyromaniacId) : BaseModifier
{
    public override string ModifierName => "Doused";
    public override bool HideOnUi => true;
    public byte PyromaniacId { get; } = pyromaniacId;

    public override void OnActivate()
    {
        var arso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.PlayerId == PyromaniacId);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PyromaniacDouse, arso!, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        if (PlayerControl.LocalPlayer.IsRole<PyromaniacRole>())
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(Color.yellow));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(Color.yellow));
    }
}