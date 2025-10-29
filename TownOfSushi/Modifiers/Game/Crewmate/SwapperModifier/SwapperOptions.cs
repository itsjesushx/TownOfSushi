using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class SwapperOptions : AbstractOptionGroup<SwapperModifier>
{
    public override string GroupName => "Swapper";
    public override uint GroupPriority => 58;
    public override Color GroupColor => TownOfSushiColors.Swapper;

    [ModdedNumberOption("Swapper Chance", 0f, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float SwapperChance { get; set; } = 1f;

    [ModdedToggleOption("Swapper Can Call Button")]
    public bool CanButton { get; set; } = true;
}