using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Neutral;

public sealed class GuardianAngelTargetModifier(byte gaId) : PlayerTargetModifier(gaId)
{
    public override string ModifierName => "Guardian Angel Target";
}