using MiraAPI.Events;
using MiraAPI.GameOptions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class MorphlingMorphModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingDuration;
    public override string ModifierName => "Morph";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Morph);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MorphlingMorph, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        Player.ResetAppearance();

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MorphlingUnmorph, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
}
