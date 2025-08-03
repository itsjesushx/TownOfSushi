﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;
namespace TownOfSushi.Options.Roles.Impostor;

public sealed class SwooperOptions : AbstractOptionGroup<SwooperRole>
{
    public override string GroupName => "Swooper";

    [ModdedNumberOption("Swoop Uses Per Round", 0f, 10f, 1f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxSwoops { get; set; } = 0f;

    [ModdedNumberOption("Swoop Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SwoopCooldown { get; set; } = 25f;

    [ModdedNumberOption("Swoop Duration", 5f, 15f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float SwoopDuration { get; set; } = 10f;

    [ModdedToggleOption("Swooper Can Vent")]
    public bool CanVent { get; set; } = true;
}
