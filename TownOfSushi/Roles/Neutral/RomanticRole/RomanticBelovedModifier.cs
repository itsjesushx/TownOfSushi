using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Neutral;

public sealed class RomanticBelovedModifier(byte romId) : PlayerTargetModifier(romId)
{
    public override string ModifierName => "Romantic Target";
}