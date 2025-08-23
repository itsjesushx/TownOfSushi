using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class MiniOptions : AbstractOptionGroup<MiniModifier>
{
    public override string GroupName => "Mini";
    public override uint GroupPriority => 26;
    public override Color GroupColor => TownOfSushiColors.Mini;

    [ModdedNumberOption("Mini Amount", 0, 5)]
    public float MiniAmount { get; set; } = 0;

    public ModdedNumberOption MiniChance { get; } = new("Mini Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<MiniOptions>.Instance.MiniAmount > 0
    };
    

    [ModdedNumberOption("Mini Speed", 1.05f, 2.5f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float MiniSpeed { get; set; } = 1.75f;
}