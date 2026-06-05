using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class WerewolfOptions : AbstractOptionGroup<WerewolfRole>
{
    public override string GroupName => "Werewolf";

    [ModdedNumberOption("Maul Cooldown", 10f, 60f, 5f, MiraNumberSuffixes.Seconds)]
    public float MaulCooldown { get; set; } = 25f;

    [ModdedToggleOption("Werewolf Can Vent")]
    public bool CanVent { get; set; } = false;

    [ModdedToggleOption("Has Impostor Vision")]
    public bool HasImpostorVision { get; set; } = false;

    [ModdedNumberOption("Maul Radius", 0.25f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float RaulRadius { get; set; } = 0.25f;
}