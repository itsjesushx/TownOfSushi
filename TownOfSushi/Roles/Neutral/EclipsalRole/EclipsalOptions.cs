using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class EclipsalOptions : AbstractOptionGroup<EclipsalRole>
{
    public override string GroupName => "Eclipsal";
    [ModdedNumberOption("Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedNumberOption("Blind Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BlindCooldown { get; set; } = 25f;

    [ModdedNumberOption("Blind Duration", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BlindDuration { get; set; } = 25f;

    [ModdedNumberOption("Blind Radius", 0.25f, 5f, 0.25f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float BlindRadius { get; set; } = 1f;
    [ModdedToggleOption("Eclipsal Can Use Vents")]
    public bool CanUseVents { get; set; } = false;
}