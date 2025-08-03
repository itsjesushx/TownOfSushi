using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Options.Roles.Neutral;

public sealed class ArsonistOptions : AbstractOptionGroup<ArsonistRole>
{
    public override string GroupName => TOSLocale.Get(TOSNames.Arsonist, "Arsonist");

    [ModdedNumberOption("Douse Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DouseCooldown { get; set; } = 25f;
    [ModdedNumberOption("Douse Duration",0f, 10f, 1f, MiraNumberSuffixes.Seconds)]
    public float DouseDuration { get; set; } = 2f;

    [ModdedToggleOption("Douse From Interactions")]
    public bool DouseInteractions { get; set; } = true;
}