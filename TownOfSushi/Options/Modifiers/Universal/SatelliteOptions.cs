﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Universal;

public sealed class SatelliteOptions : AbstractOptionGroup<SatelliteModifier>
{
    public override string GroupName => "Satellite";
    public override uint GroupPriority => 27;
    public override Color GroupColor => TownOfSushiColors.Satellite;

    [ModdedNumberOption("Button Cooldown", 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 15f;

    [ModdedNumberOption("Max Uses", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxNumCast { get; set; } = 5f;

    [ModdedToggleOption("One Usage Per Round")]
    public bool OneUsePerRound { get; set; } = true;
    
    [ModdedToggleOption("Allow Usage in First Round")]
    public bool FirstRoundUse { get; set; } = true;
}
