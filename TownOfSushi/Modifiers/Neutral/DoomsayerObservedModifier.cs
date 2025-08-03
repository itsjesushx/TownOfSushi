using MiraAPI.Modifiers;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class DoomsayerObservedModifier : BaseModifier
{
    public override string ModifierName => "Observed";
    public override bool HideOnUi => true;
}
