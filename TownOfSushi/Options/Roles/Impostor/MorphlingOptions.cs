﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class MorphlingOptions : AbstractOptionGroup<MorphlingRole>
{
    public override string GroupName => "Morphling";

    [ModdedNumberOption("Samples Per Game", 0f, 15f, 5f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxSamples { get; set; } = 0f;
    [ModdedNumberOption("Morph Uses Per Round", 0f, 10f, 1f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxMorphs { get; set; } = 0f;

    [ModdedNumberOption("Morphling Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float MorphlingCooldown { get; set; } = 25f;

    [ModdedNumberOption("Morphling Duration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float MorphlingDuration { get; set; } = 10f;

    [ModdedToggleOption("Morphling Can Vent")]
    public bool MorphlingVent { get; set; } = true;
}
