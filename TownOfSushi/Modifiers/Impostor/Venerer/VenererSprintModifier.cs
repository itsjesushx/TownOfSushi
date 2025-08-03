using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Impostor;

namespace TownOfSushi.Modifiers.Impostor.Venerer;

public sealed class VenererSprintModifier : TimedModifier, IVenererModifier
{
    public override string ModifierName => "Sprint";
    public override bool AutoStart => true;
    public override float Duration => OptionGroupSingleton<VenererOptions>.Instance.AbilityDuration;
    public override void OnActivate()
    {
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.VenererSprintAbility, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
}
