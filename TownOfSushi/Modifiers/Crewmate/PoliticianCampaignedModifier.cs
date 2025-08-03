using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TosEvents;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class PoliticianCampaignedModifier(PlayerControl politician) : BaseModifier
{
    public override string ModifierName => "Campaigned";
    public override bool HideOnUi => true;

    public PlayerControl Politician { get; } = politician;
    public override void OnActivate()
    {
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.PoliticianCampaign, Politician, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}
