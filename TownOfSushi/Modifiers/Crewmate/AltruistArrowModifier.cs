﻿using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class AltruistArrowModifier(PlayerControl owner, Color color) : ArrowTargetModifier(owner, color, 0)
{
    public override string ModifierName => "Altruist Arrow";
}
