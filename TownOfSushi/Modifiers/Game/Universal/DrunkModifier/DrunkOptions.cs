using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class DrunkOptions : AbstractOptionGroup<DrunkModifier>
{
    public override string GroupName => "Drunk";
    public override uint GroupPriority => 36;
    public override Color GroupColor => TownOfSushiColors.Drunk;
    [ModdedNumberOption("Drunk Amount", 0, 1, 1)]
    public float DrunkAmount { get; set; } = 0;
    public ModdedNumberOption DrunkChance { get; } = new ModdedNumberOption("Drunk Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<DrunkOptions>.Instance.DrunkAmount > 0,
    };

    [ModdedNumberOption("Max Rounds Being Drunk", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float DrunkDuration { get; set; } = 5f;
}