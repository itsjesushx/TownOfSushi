using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class GiantOptions : AbstractOptionGroup<GiantModifier>
{
    public override string GroupName => "Giant";
    public override uint GroupPriority => 25;
    public override Color GroupColor => TownOfSushiColors.Giant;
    [ModdedNumberOption("Giant Amount", 0, 5)]
    public float GiantAmount { get; set; } = 0;

    public ModdedNumberOption GiantChance { get; } = new("Giant Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<GiantOptions>.Instance.GiantAmount > 0
    };

    [ModdedNumberOption("Giant Speed", 0.25f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float GiantSpeed { get; set; } = 0.75f;
}