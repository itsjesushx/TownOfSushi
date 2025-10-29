﻿using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Impostor;

public sealed class BomberOptions : AbstractOptionGroup<BomberRole>
{
    public override string GroupName => "Bomber";

    [ModdedNumberOption("Bomb Uses Per Game", 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxBombs { get; set; } = 3f;

    [ModdedNumberOption("Detonate Delay", 1f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float DetonateDelay { get; set; } = 5f;

    [ModdedNumberOption("Detonate Radius", 0.05f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float DetonateRadius { get; set; } = 0.25f;

    [ModdedNumberOption("Max Kills In Detonation", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxKillsInDetonation { get; set; } = 5f;

    [ModdedToggleOption("All Impostors See Bomb")]
    public bool AllImpsSeeBomb { get; set; } = true;

    [ModdedToggleOption("Bomber Can Vent")]
    public bool BomberVent { get; set; } = true;
}