using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class PoliticianCampaignedModifier(PlayerControl politician) : BaseModifier
{
    public override string ModifierName => "Campaigned";
    public override bool HideOnUi => true;

    public PlayerControl Politician { get; } = politician;

    public override void OnActivate()
    {
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.PoliticianCampaign, Politician, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }
}