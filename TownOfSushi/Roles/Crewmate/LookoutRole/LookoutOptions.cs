using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class LookoutOptions : AbstractOptionGroup<LookoutRole>
{
    public override string GroupName => "Lookout";

    [ModdedNumberOption("Watch Cooldown", 1f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float WatchCooldown { get; set; } = 20f;

    [ModdedNumberOption("Watch Duration", 3f, 10f, 1f, MiraNumberSuffixes.Seconds)]
    public float WatchDuration { get; set; } = 5f;

    [ModdedToggleOption("Get More Uses From Completing Tasks")]
    public bool TaskUses { get; set; } = false;
    [ModdedNumberOption("Max Number Of Watches", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxWatches { get; set; } = 5f;
}