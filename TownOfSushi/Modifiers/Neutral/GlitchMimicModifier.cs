using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class GlitchMimicModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<GlitchOptions>.Instance.MimicDuration;
    public override string ModifierName => "Mimic";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Mimic);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.GlitchMimic, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        CustomButtonSingleton<GlitchMimicButton>.Instance.SetTimer(OptionGroupSingleton<GlitchOptions>.Instance.MimicCooldown);
        Player.ResetAppearance();
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.GlitchUnmimic, Player, target);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
}
