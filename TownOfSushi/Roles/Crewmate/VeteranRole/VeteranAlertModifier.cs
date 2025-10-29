using MiraAPI.Events;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VeteranAlertModifier : TimedModifier
{
    public override float Duration => OptionGroupSingleton<VeteranOptions>.Instance.AlertDuration;
    public override string ModifierName => "Alerted";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        base.OnActivate();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.VeteranAlert, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);

        if (Player.Data.Role is VeteranRole vet)
        {
            vet.Alerts--;
        }
    }
}