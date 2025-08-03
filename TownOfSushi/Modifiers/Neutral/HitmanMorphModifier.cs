using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class HitmanMorphModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<AgentOptions>.Instance.MorphDuration;
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
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.HitmanMorph, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        CustomButtonSingleton<HitmanMorphButton>.Instance.SetTimer(OptionGroupSingleton<AgentOptions>.Instance.MorphCooldown);
        Player.ResetAppearance();
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.HitmanUnMorph, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
}