using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class ScavengerOptions : AbstractOptionGroup<ScavengerRole>
{
    public override string GroupName => "Scavenger";

    [ModdedNumberOption("Bodies Needed To Win", 2f, 10f, 1f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float EatNeed { get; set; } = 4f;

    [ModdedNumberOption("Eat Cooldown", 10f, 60f, 5f, MiraNumberSuffixes.Seconds)]
    public float EatCooldown { get; set; } = 25f;

    [ModdedNumberOption("Eat Duration", 0f, 60f, 1f, MiraNumberSuffixes.Seconds)]
    public float EatDuration { get; set; } = 2f;

    [ModdedToggleOption("Scavenger Can Vent")]
    public bool CanVent { get; set; } = false;

    [ModdedToggleOption("Scavenger Has Impostor Vision")]
    public bool HasImpostorVision { get; set; } = false;

    [ModdedNumberOption("Scavenge Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ScavengeCooldown { get; set; } = 30f;

    [ModdedNumberOption("Scavenge Duration", 1f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float ScavengeDuration { get; set; } = 7f;

    [ModdedNumberOption("Max Scavenge Uses", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxScavengeUses { get; set; } = 5f;    
}