﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class WarlockOptions : AbstractOptionGroup<WarlockRole>
{
    public override string GroupName => "Warlock";

    [ModdedNumberOption("Time It Takes To Fully Charge", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ChargeTimeDuration { get; set; } = 25f;
    [ModdedNumberOption("Time Multiplier Added Per Kill For Next Charge", 0f, 0.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float AddedTimeDuration { get; set; } = 0.05f;

    [ModdedNumberOption("Time It Takes To Use Full Charge", 0.05f, 5f, 0.05f, MiraNumberSuffixes.Seconds, "0.00")]
    public float DischargeTimeDuration { get; set; } = 1f;
}
