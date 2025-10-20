using MiraAPI.Events;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Roles.Neutral;

public sealed class AmnesiacVestModifier : TimedModifier
{
    public override float Duration => OptionGroupSingleton<AmnesiacOptions>.Instance.VestDuration;
    public override string ModifierName => "Vested";
    public override bool AutoStart => true;
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        base.OnActivate();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.AmnesiacVest, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }
}