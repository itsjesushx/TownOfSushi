using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ShyOptions : AbstractOptionGroup<ShyModifier>
{
    public override string GroupName => "Shy";
    public override uint GroupPriority => 28;
    public override Color GroupColor => TownOfSushiColors.Shy;

    [ModdedNumberOption("Shy Amount", 0, 5)]
    public float ShyAmount { get; set; } = 0;

    public ModdedNumberOption ShyChance { get; } = new("Shy Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<ShyOptions>.Instance.ShyAmount > 0
    };

    [ModdedNumberOption("Transparency Delay", 0f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float InvisDelay { get; set; } = 5f;

    [ModdedNumberOption("Turn Transparent Duration", 0f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float TransformInvisDuration { get; set; } = 5f;

    [ModdedNumberOption("Final Opacity", 0f, 80f, 10f, MiraNumberSuffixes.Percent)]
    public float FinalTransparency { get; set; } = 20f;
}