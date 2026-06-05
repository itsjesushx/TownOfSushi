using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ParanoiacOptions : AbstractOptionGroup<ParanoiacModifier>
{
    public override string GroupName => "Paranoiac";
    public override uint GroupPriority => 52;
    public override Color GroupColor => TownOfSushiColors.Paranoiac;
    [ModdedNumberOption("Paranoiac Amount", 0, 5)]
    public float ParanoiacAmount { get; set; } = 0;

    public ModdedNumberOption ParanoiacChance { get; } = new("Paranoiac Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<ParanoiacOptions>.Instance.ParanoiacAmount > 0
    };
}