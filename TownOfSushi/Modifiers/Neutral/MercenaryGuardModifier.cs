using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TosEvents;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class MercenaryGuardModifier(PlayerControl mercenary) : BaseModifier
{
    public override string ModifierName => "Mercenary Guard";
    public override bool HideOnUi => true;
    public PlayerControl Mercenary { get; } = mercenary;

    public override void OnActivate()
    {
        base.OnActivate();

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MercenaryGuard, Mercenary, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}
