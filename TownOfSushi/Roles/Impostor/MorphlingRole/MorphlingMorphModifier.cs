using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Roles.Impostor;

public sealed class MorphlingMorphModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingDuration;
    public override string ModifierName => "Morph";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public bool VisualPriority => true;
    public override bool VisibleToOthers => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Morph);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.MorphlingMorph, Player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.MorphlingUnmorph, Player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}