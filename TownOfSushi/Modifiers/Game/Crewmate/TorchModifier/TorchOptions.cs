using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public sealed class TorchOptions : AbstractOptionGroup<TorchModifier>
{
    public override string GroupName => "Torch";
    public override Color GroupColor => TownOfSushiColors.Torch;
    public override uint GroupPriority => 49;

    [ModdedNumberOption("Torch Amount", 0, 5)]
    public float TorchAmount { get; set; } = 0;

    public ModdedNumberOption TorchChance { get; } =
        new("Torch Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TorchOptions>.Instance.TorchAmount > 0
        };
}