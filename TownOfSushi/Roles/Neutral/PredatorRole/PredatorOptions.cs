using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class PredatorOptions : AbstractOptionGroup<PredatorRole>
{
    public override string GroupName => "Predator";

    [ModdedNumberOption("Terminate Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TerminateCooldown { get; set; } = 25f;

    [ModdedNumberOption("Terminate Duration", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TerminateDuration { get; set; } = 10f;

    [ModdedNumberOption("Terminate Kill Cooldown", 0.5f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float TerminateKillCooldown { get; set; } = 1.5f;

    [ModdedToggleOption("Predator Can Vent When Terminated")]
    public bool CanVent { get; set; } = true;
}