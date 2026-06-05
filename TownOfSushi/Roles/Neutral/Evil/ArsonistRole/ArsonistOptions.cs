using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Neutral;

public sealed class ArsonistOptions : AbstractOptionGroup<ArsonistRole>
{
    public override string GroupName => "Arsonist";

    [ModdedNumberOption("Douse Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DouseCooldown { get; set; } = 25f;
    [ModdedNumberOption("Douse Duration",0f, 10f, 1f, MiraNumberSuffixes.Seconds)]
    public float DouseDuration { get; set; } = 2f;

    [ModdedToggleOption("Douse From Interactions")]
    public bool DouseInteractions { get; set; } = true;
}