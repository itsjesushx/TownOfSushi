using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Options.Roles.Impostor;

public sealed class ViperOptions : AbstractOptionGroup<ViperRole>
{
    public override string GroupName => "Viper";

    [ModdedNumberOption("Blind Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BlindCooldown { get; set; } = 25f;
    [ModdedNumberOption("Number Of Blind Traps Per Game", 0f, 30f, 5f, MiraNumberSuffixes.None, "0", zeroInfinity: true)]
    public float MaxBlindTraps { get; set; } = 0f;

    [ModdedNumberOption("Blind Duration", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float BlindDuration { get; set; } = 25f;

    [ModdedNumberOption("Poison Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PoisonCooldown { get; set; } = 25f;

    [ModdedNumberOption("Poison Delay", 0f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PoisonDelay { get; set; } = 5f;

    [ModdedToggleOption("Can Use Vents")]
    public bool CanUseVents { get; set; } = false;
}