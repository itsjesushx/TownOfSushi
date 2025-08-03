using UnityEngine;

namespace TownOfSushi.Modifiers.Neutral;

public sealed class ScavengerArrowModifier(DeadBody deadBody, Color color) : ArrowDeadBodyModifier(deadBody, color, 0)
{
    public override string ModifierName => "Scavenger Arrow";
}