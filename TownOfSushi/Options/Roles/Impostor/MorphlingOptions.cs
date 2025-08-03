﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;

using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class MorphlingOptions : AbstractOptionGroup<MorphlingRole>
{
    public override string GroupName => TOSLocale.Get(TOSNames.Morphling, "Morphling");

    [ModdedNumberOption("Samples Per Game", 0f, 15f, 5f, MiraNumberSuffixes.None, "0", true)]
    public float MaxSamples { get; set; } = 0f;

    [ModdedNumberOption("Morph Uses Per Round", 0f, 10f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxMorphs { get; set; } = 0f;

    [ModdedNumberOption("Morph Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float MorphlingCooldown { get; set; } = 25f;

    [ModdedNumberOption("Morph Duration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float MorphlingDuration { get; set; } = 10f;

    [ModdedToggleOption("Morph Can Vent")]
    public bool MorphlingVent { get; set; } = true;
}