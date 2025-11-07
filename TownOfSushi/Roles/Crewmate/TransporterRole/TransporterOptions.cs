using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class TransporterOptions : AbstractOptionGroup<TransporterRole>
{
    public override string GroupName => "Transporter";

    [ModdedNumberOption("Transport Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TransporterCooldown { get; set; } = 25f;

    [ModdedNumberOption("Max Uses", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxNumTransports { get; set; } = 5f;

    [ModdedToggleOption("Move While Using Transport Menu (KB ONLY)")]
    public bool MoveWithMenu { get; set; } = true;

    [ModdedToggleOption("Can Use Vitals")]
    public bool CanUseVitals { get; set; } = true;

    [ModdedToggleOption("Get More Uses From Completing Tasks")]
    public bool TaskUses { get; set; } = true;

    [ModdedToggleOption("Transporter Can Transport Themselves Without Anyone Else")]
    public bool TransportSelf { get; set; } = false;

    [ModdedNumberOption("Transporter Recall Uses Per Game", 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxEscapes { get; set; } = 0f;

    [ModdedNumberOption("Transporter Recall Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float RecallCooldown { get; set; } = 25f;
}