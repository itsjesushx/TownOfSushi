using MiraAPI.GameOptions.Attributes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public sealed class EgotistOptions : AbstractOptionGroup<EgotistModifier>
{
    public override string GroupName => "Egotist";
    public override Color GroupColor => TownOfSushiColors.Egotist;
    public override uint GroupPriority => 37;

    [ModdedNumberOption("Egotist Chance", 0, 100f, 10f, MiraNumberSuffixes.Percent)]
    public float EgotistChance { get; set; } = 0;
}