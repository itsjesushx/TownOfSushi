using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class LazyOptions : AbstractOptionGroup<LazyModifier>
{
    public override string GroupName => "Lazy";
    public override uint GroupPriority => 51;
    public override Color GroupColor => TownOfSushiColors.Lazy;
    [ModdedNumberOption("Lazy Amount", 0, 5)]
    public float LazyAmount { get; set; } = 0;

    public ModdedNumberOption LazyChance { get; } = new("Lazy Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<LazyOptions>.Instance.LazyAmount > 0
    };
}