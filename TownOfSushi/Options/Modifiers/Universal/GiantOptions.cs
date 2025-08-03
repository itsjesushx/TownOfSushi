using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Universal;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Universal;

public sealed class GiantOptions : AbstractOptionGroup<GiantModifier>
{
    public override string GroupName => "Giant";
    public override uint GroupPriority => 25;
    public override Color GroupColor => TownOfSushiColors.Giant;

    [ModdedNumberOption("Giant Speed", 0.25f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float GiantSpeed { get; set; } = 0.75f;
}