using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class DetectiveOptions : AbstractOptionGroup<DetectiveRole>
{
    public override string GroupName => "Detective";

    [ModdedNumberOption("Detective Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DetectiveCooldown { get; set; } = 25f;

    [ModdedToggleOption("Crewmate Killing Roles Are Red")]
    public bool ShowCrewmateKillingAsRed { get; set; } = false;

    [ModdedToggleOption("Neutral Benign Roles Are Red")]
    public bool ShowNeutralBenignAsRed { get; set; } = false;

    [ModdedToggleOption("Neutral Evil Roles Are Red")]
    public bool ShowNeutralEvilAsRed { get; set; } = false;

    [ModdedToggleOption("Neutral Killing Roles Are Red")]
    public bool ShowNeutralKillingAsRed { get; set; } = true;

    [ModdedToggleOption("Traitor Swaps Colors")]
    public bool SwapTraitorColors { get; set; } = true;
}