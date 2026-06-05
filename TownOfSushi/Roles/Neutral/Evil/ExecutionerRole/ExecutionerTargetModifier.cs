using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Neutral;

public sealed class ExecutionerTargetModifier(byte exeId) : PlayerTargetModifier(exeId)
{
    public override string ModifierName => "Executioner Target";
}