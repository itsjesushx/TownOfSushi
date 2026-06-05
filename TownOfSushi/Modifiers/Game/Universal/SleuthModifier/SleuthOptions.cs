using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SleuthOptions : AbstractOptionGroup<SleuthModifier>
{
    public override string GroupName => "Sleuth";
    public override uint GroupPriority => 54;
    public override Color GroupColor => TownOfSushiColors.Sleuth;
    [ModdedNumberOption("Sleuth Amount", 0, 5)]
    public float SleuthAmount { get; set; } = 0;

    public ModdedNumberOption SleuthChance { get; } = new("Sleuth Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<SleuthOptions>.Instance.SleuthAmount > 0
    };
}