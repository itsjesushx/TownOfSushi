﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers;

public sealed class AllianceModifierOptions : AbstractOptionGroup
{
    public override string GroupName => "Alliance Modifiers";
    public override Color GroupColor => Color.white;
    public override bool ShowInModifiersMenu => true;
    public override uint GroupPriority => 0;

    [ModdedNumberOption("Egotist Chance", 0, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float EgotistChance { get; set; } = 0;
    
    [ModdedNumberOption("Lovers Chance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float LoversChance { get; set; } = 0;
}
