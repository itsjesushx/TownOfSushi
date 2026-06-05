using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class MultitaskerOptions : AbstractOptionGroup<MultitaskerModifier>
{
    public override string GroupName => "Multitasker";
    public override uint GroupPriority => 46;
    public override Color GroupColor => TownOfSushiColors.Multitasker;

    [ModdedNumberOption("Multitasker Amount", 0, 5)]
    public float MultitaskerAmount { get; set; } = 0;

    public ModdedNumberOption MultitaskerChance { get; } =
        new("Multitasker Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<MultitaskerOptions>.Instance.MultitaskerAmount > 0
        };
}