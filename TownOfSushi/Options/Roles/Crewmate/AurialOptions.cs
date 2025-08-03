using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class AurialOptions : AbstractOptionGroup<AurialRole>
{
    public override string GroupName => "Aurial";

    [ModdedNumberOption("Radiate Colour Range", 0f, 1f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float AuraInnerRadius { get; set; } = 0.5f;

    [ModdedNumberOption("Radiate Max Range", 1f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float AuraOuterRadius { get; set; } = 1.5f;

    [ModdedNumberOption("Sense Duration", 1f, 15f, 1f, MiraNumberSuffixes.Seconds, "0")]
    public float SenseDuration { get; set; } = 10f;
}