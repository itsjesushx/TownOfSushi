using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class OracleBlessedModifier(PlayerControl oracle) : BaseModifier
{
    public override string ModifierName => "Blessed";
    public override bool HideOnUi => true;
    public PlayerControl Oracle { get; } = oracle;

    public bool SavedFromExile { get; set; }

    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.OracleBless, Oracle, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}