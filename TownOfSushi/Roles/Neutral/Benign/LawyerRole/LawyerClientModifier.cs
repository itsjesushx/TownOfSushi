using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Neutral;

public sealed class LawyerClientModifier(byte romId) : PlayerTargetModifier(romId)
{
    public override string ModifierName => "Lawyer Client";
}