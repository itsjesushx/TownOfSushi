using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class TiebreakerOptions : AbstractOptionGroup<TiebreakerModifier>
{
    public override string GroupName => "Tiebreaker";
    public override uint GroupPriority => 55;
    public override Color GroupColor => TownOfSushiColors.Tiebreaker;
    [ModdedNumberOption("Tiebreaker Amount", 0, 5)]
    public float TiebreakerAmount { get; set; } = 0;

    public ModdedNumberOption TiebreakerChance { get; } = new("Tiebreaker Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<TiebreakerOptions>.Instance.TiebreakerAmount > 0
    };
}