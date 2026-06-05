using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MonarchOptions : AbstractOptionGroup<MonarchRole>
{
    public override string GroupName => "Monarch";

    [ModdedNumberOption("Knight Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds, "0.0")]
    public float KnightCooldown { get; set; } = 25f;

    [ModdedNumberOption("Max Number Of Knights", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxKnights { get; set; } = 3f;

    [ModdedToggleOption("Get More Charges From Completing Tasks")]
    public bool TaskUses { get; set; } = true;
}