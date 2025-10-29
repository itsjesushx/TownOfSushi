using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DiseasedOptions : AbstractOptionGroup<DiseasedModifier>
{
    public override string GroupName => "Diseased";
    public override uint GroupPriority => 33;
    public override Color GroupColor => TownOfSushiColors.Diseased;

    [ModdedNumberOption("Diseased Amount", 0, 5)]
    public float DiseasedAmount { get; set; } = 0;

    public ModdedNumberOption DiseasedChance { get; } =
        new("Diseased Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<DiseasedOptions>.Instance.DiseasedAmount > 0
        };
        

    [ModdedNumberOption("Diseased Kill Multiplier", 1.5f, 5f, 0.5f, MiraNumberSuffixes.Multiplier)]
    public float CooldownMultiplier { get; set; } = 3f;
}