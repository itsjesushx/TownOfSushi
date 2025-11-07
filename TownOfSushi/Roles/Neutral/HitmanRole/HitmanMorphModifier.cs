using MiraAPI.Events;
using MiraAPI.Hud;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Roles.Neutral;

public sealed class HitmanMorphModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<AgentOptions>.Instance.MorphDuration;
    public override string ModifierName => "Hitman Morph";
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
        var TosAbilityEvent = new TOSAbilityEvent(AbilityType.HitmanMorph, Player, target);
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
        var TosAbilityEvent = new TOSAbilityEvent(AbilityType.HitmanUnMorph, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
}