using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class CrusaderOptions : AbstractOptionGroup<CrusaderRole>
{
    public override string GroupName => "Crusader";

    [ModdedEnumOption("Show Fortify Player", typeof(FortifyOptions), ["Fortified", "Crusader", "Fortified + Crusader", "Everyone"])]
    public FortifyOptions ShowFortified { get; set; } = FortifyOptions.SelfAndCrusader;
}

public enum FortifyOptions
{
    Self,
    Crusader,
    SelfAndCrusader,
    Everyone
}