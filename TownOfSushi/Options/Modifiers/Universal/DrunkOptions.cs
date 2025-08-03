using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Universal;

public sealed class DrunkOptions : AbstractOptionGroup<DrunkModifier>
{
    public override string GroupName => "Drunk";
    public override uint GroupPriority => 29;
    public override Color GroupColor => TownOfSushiColors.Drunk;

    [ModdedNumberOption("Max Rounds Being Drunk", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float DrunkDuration { get; set; } = 5f;
}