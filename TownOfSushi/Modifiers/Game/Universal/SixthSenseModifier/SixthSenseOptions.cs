using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SixthSenseOptions : AbstractOptionGroup<SixthSenseModifier>
{
    public override string GroupName => "Sixth Sense";
    public override uint GroupPriority => 53;
    public override Color GroupColor => TownOfSushiColors.SixthSense;
    [ModdedNumberOption("Sixth Sense Amount", 0, 5)]
    public float SixthSenseAmount { get; set; } = 0;

    public ModdedNumberOption SixthSenseChance { get; } = new("Sixth Sense Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<SixthSenseOptions>.Instance.SixthSenseAmount > 0
    };
}