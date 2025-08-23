using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Impostor;

public sealed class ViperOptions : AbstractOptionGroup<ViperRole>
{
    public override string GroupName => "Viper";

    [ModdedNumberOption("Poison Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PoisonCooldown { get; set; } = 25f;

    [ModdedNumberOption("Poison Delay", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PoisonDelay { get; set; } = 5f;

    [ModdedToggleOption("Can Use Vents")]
    public bool CanUseVents { get; set; } = false;
}