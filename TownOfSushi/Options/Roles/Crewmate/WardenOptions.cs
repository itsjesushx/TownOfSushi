﻿using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class WardenOptions : AbstractOptionGroup<WardenRole>
{
    public override string GroupName => "Warden";

    [ModdedEnumOption("Show Fortify Player", typeof(FortifyOptions), ["Self", "Warden", "Self + Warden", "Everyone"])]
    public FortifyOptions ShowFortified { get; set; } = FortifyOptions.SelfAndWarden;
}

public enum FortifyOptions
{
    Self,
    Warden,
    SelfAndWarden,
    Everyone,
}
