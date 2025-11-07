using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Neutral;

public sealed class PyromaniacOptions : AbstractOptionGroup<PyromaniacRole>
{
    public override string GroupName => "Pyromaniac";

    [ModdedNumberOption("Douse Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DouseCooldown { get; set; } = 25f;

    [ModdedToggleOption("Douse From Interactions")]
    public bool DouseInteractions { get; set; } = true;

    [ModdedToggleOption("Legacy Mode (No Radius)")]
    public bool LegacyPyromaniac { get; set; } = true;

    public ModdedNumberOption IgniteRadius { get; set; } = new("Ignite Radius", 0.25f, 0.05f, 1f, 0.05f,
        MiraNumberSuffixes.Multiplier, "0.00")
    {
        Visible = () => !OptionGroupSingleton<PyromaniacOptions>.Instance.LegacyPyromaniac
    };

    [ModdedToggleOption("Pyromaniac Can Vent")]
    public bool CanVent { get; set; }
}