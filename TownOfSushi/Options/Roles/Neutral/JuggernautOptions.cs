﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Options.Roles.Neutral;

public sealed class JuggernautOptions : AbstractOptionGroup<JuggernautRole>
{
    public override string GroupName => "Juggernaut";

    [ModdedNumberOption("Initial Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedNumberOption("Kill Cooldown Reduction", 2.5f, 15f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownReduction { get; set; } = 5f;

    [ModdedToggleOption("Juggernaut Can Vent")]
    public bool CanVent { get; set; } = true;
}
