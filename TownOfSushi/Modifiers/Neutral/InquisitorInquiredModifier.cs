using MiraAPI.Modifiers;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class InquisitorInquiredModifier : BaseModifier
{
    public override string ModifierName => "Inquired";
    public override bool HideOnUi => true;
}