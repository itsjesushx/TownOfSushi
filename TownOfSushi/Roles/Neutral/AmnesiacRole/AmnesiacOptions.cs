using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class AmnesiacOptions : AbstractOptionGroup<AmnesiacRole>
{
    public override string GroupName => "Amnesiac";

    [ModdedNumberOption("Vest Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float VestCooldown { get; set; } = 25f;

    [ModdedNumberOption("Vest Duration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float VestDuration { get; set; } = 10f;

    [ModdedNumberOption("Max Number Of Vests", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxVests { get; set; } = 10f;

    [ModdedToggleOption("Amnesiac Scatter Mechanic Enabled")]
    public bool ScatterOn { get; set; } = false;

    [ModdedNumberOption("Amnesiac Scatter Timer", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float ScatterTimer { get; set; } = 25f;
}