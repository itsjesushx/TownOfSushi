﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class HypnotistOptions : AbstractOptionGroup<HypnotistRole>
{
    public override string GroupName => "Hypnotist";

    [ModdedNumberOption("Hypnotize Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float HypnotiseCooldown { get; set; } = 25f;

    [ModdedToggleOption("Hypnotist Can Kill With Teammate")]
    public bool HypnoKill { get; set; } = true;
}