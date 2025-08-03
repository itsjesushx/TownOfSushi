﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Universal;

public sealed class FlashOptions : AbstractOptionGroup<FlashModifier>
{
    public override string GroupName => "Flash";
    public override uint GroupPriority => 24;
    public override Color GroupColor => TownOfSushiColors.Flash;

    [ModdedNumberOption("Flash Speed", 1.05f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float FlashSpeed { get; set; } = 1.75f;
}
