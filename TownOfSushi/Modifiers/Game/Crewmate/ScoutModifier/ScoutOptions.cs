using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public sealed class ScoutOptions : AbstractOptionGroup<ScoutModifier>
{
    public override string GroupName => "Scout";
    public override Color GroupColor => TownOfSushiColors.Scout;
    public override uint GroupPriority => 48;

    [ModdedNumberOption("Scout Amount", 0, 5)]
    public float ScoutAmount { get; set; } = 0;

    public ModdedNumberOption ScoutChance { get; } =
        new("Scout Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<ScoutOptions>.Instance.ScoutAmount > 0
        };
}