using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class RetributionistOptions : AbstractOptionGroup<RetributionistRole>
{
    public override string GroupName => "Retributionist";

    [ModdedNumberOption("Revive Duration", 1f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float ReviveDuration { get; set; } = 10f;

    [ModdedNumberOption("Revive Range", 0.05f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float ReviveRange { get; set; } = 0.25f;

    [ModdedNumberOption("Revive Uses", 1f, 5f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxRevives { get; set; } = 2;

    [ModdedToggleOption("Hide Bodies at Beginning Of Revive")]
    public bool HideAtBeginningOfRevive { get; set; } = false;
}