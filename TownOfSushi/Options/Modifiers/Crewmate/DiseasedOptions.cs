using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Crewmate;

public sealed class DiseasedOptions : AbstractOptionGroup<DiseasedModifier>
{
    public override string GroupName => "Diseased";
    public override uint GroupPriority => 32;
    public override Color GroupColor => TownOfSushiColors.Diseased;

    [ModdedNumberOption("Diseased Kill Multiplier", 1.5f, 5f, 0.5f, MiraNumberSuffixes.Multiplier)]
    public float CooldownMultiplier { get; set; } = 3f;
}
