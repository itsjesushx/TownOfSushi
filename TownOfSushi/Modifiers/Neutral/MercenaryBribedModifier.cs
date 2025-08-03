using MiraAPI.Events;
using MiraAPI.Modifiers;
using Reactor.Utilities.Extensions;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Utilities;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class MercenaryBribedModifier(PlayerControl mercenary) : BaseModifier
{
    public override string ModifierName => "Mercenary Bribed";
    public override bool HideOnUi => true;
    public PlayerControl Mercenary { get; } = mercenary;

    public bool alerted;

    public override void OnActivate()
    {
        base.OnActivate();

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.MercenaryBribe, Mercenary, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }


    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void OnMeetingStart()
    {
        if (!Player.AmOwner) return;
        if (alerted) return;

        var title = $"<color=#{TownOfSushiColors.Mercenary.ToHtmlStringRGBA()}>Mercenary Feedback</color>";
        MiscUtils.AddFakeChat(Player.Data, title, "You have been bribed by a Mercenary!", false, true);

        alerted = true;
    }
}
