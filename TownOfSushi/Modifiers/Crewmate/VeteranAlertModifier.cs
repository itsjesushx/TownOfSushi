using MiraAPI.Events;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class VeteranAlertModifier : TimedModifier
{
    public override float Duration => OptionGroupSingleton<VeteranOptions>.Instance.AlertDuration;
    public override string ModifierName => "Alerted";
    public override bool HideOnUi => true;

    public override void OnActivate()
    {
        base.OnActivate();

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.VeteranAlert, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);

        if (Player.Data.Role is VeteranRole vet)
        {
            vet.Alerts--;
        }
    }
}
