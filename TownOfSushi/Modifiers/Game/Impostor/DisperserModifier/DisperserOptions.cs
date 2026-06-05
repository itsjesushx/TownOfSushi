using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class DisperserOptions : AbstractOptionGroup<DisperserModifier>
{
    public override string GroupName => "Disperser";
    public override Color GroupColor => TownOfSushiColors.Impostor;
    public override uint GroupPriority => 50;

    [ModdedNumberOption("Disperser Amount", 0, 5)]
    public float DisperserAmount { get; set; } = 0;

    public ModdedNumberOption DisperserChance { get; } =
        new("Disperser Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<DisperserOptions>.Instance.DisperserAmount > 0
        };
}