using MiraAPI.Events;
using MiraAPI.Hud;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;

namespace TownOfSushi.Roles.Neutral;

public sealed class GlitchMimicModifier(PlayerControl target) : ConcealedModifier, IVisualAppearance
{
    public override float Duration => OptionGroupSingleton<GlitchOptions>.Instance.MimicDuration;
    public override string ModifierName => "Mimic";
    public override bool HideOnUi => true;
    public override bool AutoStart => true;
    public override bool VisibleToOthers => true;
    public bool VisualPriority => true;

    public VisualAppearance GetVisualAppearance()
    {
        return new VisualAppearance(target.GetDefaultModifiedAppearance(), TownOfSushiAppearances.Mimic);
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.GlitchMimic, Player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnDeactivate()
    {
        CustomButtonSingleton<GlitchMimicButton>.Instance.SetTimer(OptionGroupSingleton<GlitchOptions>.Instance
            .MimicCooldown);
        Player.ResetAppearance();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.GlitchUnmimic, Player, target);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}