using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Options.Roles.Neutral;

public sealed class ArsonistOptions : AbstractOptionGroup<ArsonistRole>
{
    public override string GroupName => "Arsonist";

    [ModdedNumberOption("Douse Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DouseCooldown { get; set; } = 25f;

    [ModdedToggleOption("Douse From Interactions")]
    public bool DouseInteractions { get; set; } = true;

    [ModdedToggleOption("Legacy Arsonist (No Radius)")]
    public bool LegacyArsonist { get; set; } = true;
    public ModdedNumberOption IgniteRadius { get; set; } = new("Ignite Radius", 0.25f, 0.05f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")
    {
        Visible = () => !OptionGroupSingleton<ArsonistOptions>.Instance.LegacyArsonist
    };

    [ModdedToggleOption("Arsonist Can Vent")]
    public bool CanVent { get; set; }
}
