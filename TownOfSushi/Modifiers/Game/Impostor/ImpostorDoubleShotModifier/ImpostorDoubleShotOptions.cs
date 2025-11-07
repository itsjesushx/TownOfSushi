using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class ImpostorDoubleShotOptions : AbstractOptionGroup<ImpostorDoubleShotModifier>
{
    public override string GroupName => "Impostor Double Shot";
    public override Color GroupColor => TownOfSushiColors.Impostor;
    public override uint GroupPriority => 50;

    [ModdedNumberOption("Impostor Double Shot Amount", 0, 5)]
    public float ImpostorDoubleShotAmount { get; set; } = 0;

    public ModdedNumberOption ImpostorDoubleShotChance { get; } =
        new("Impostor Double Shot Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<ImpostorDoubleShotOptions>.Instance.ImpostorDoubleShotAmount > 0
        };
}