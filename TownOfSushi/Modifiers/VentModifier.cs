﻿using MiraAPI.Modifiers;

namespace TownOfSushi.Modifiers;

public sealed class VentModifier : BaseModifier
{
    public override string ModifierName => "Vent";
    public override bool HideOnUi => true;

    public override bool? CanVent()
    {
        return true;
    }
}