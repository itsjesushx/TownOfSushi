using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Crewmate;

namespace TownOfSushi.Options.Roles.Crewmate;

public sealed class LookoutOptions : AbstractOptionGroup<LookoutRole>
{
    public override string GroupName => "Lookout";

    [ModdedNumberOption("Watch Cooldown", 1f, 30f, 1f, MiraNumberSuffixes.Seconds)]
    public float WatchCooldown { get; set; } = 20f;

    [ModdedNumberOption("Max Players That Can Be Watched", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxWatches { get; set; } = 5;

    [ModdedToggleOption("Lookout Watches Reset After Each Round")]
    public bool LoResetOnNewRound { get; set; } = true;

    public ModdedToggleOption TaskUses { get; } = new("Get More Uses From Completing Tasks", false)
    {
        Visible = () => !OptionGroupSingleton<LookoutOptions>.Instance.LoResetOnNewRound
    };
}