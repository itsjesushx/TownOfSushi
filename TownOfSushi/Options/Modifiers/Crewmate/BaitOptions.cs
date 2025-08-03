﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Crewmate;

public sealed class BaitOptions : AbstractOptionGroup<BaitModifier>
{
    public override string GroupName => "Bait";
    public override uint GroupPriority => 31;
    public override Color GroupColor => TownOfSushiColors.Bait;

    [ModdedNumberOption("Min Bait Report Delay", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float MinDelay { get; set; } = 0f;

    [ModdedNumberOption("Max Bait Report Delay", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float MaxDelay { get; set; } = 1f;
}