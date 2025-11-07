using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ButtonBarryOptions : AbstractOptionGroup<ButtonBarryModifier>
{
    public override string GroupName => "Button Barry";
    public override uint GroupPriority => 22;
    public override Color GroupColor => TownOfSushiColors.ButtonBarry;

    [ModdedNumberOption("Button Barry Amount", 0, 1)]
    public float ButtonBarryAmount { get; set; } = 0;

    public ModdedNumberOption ButtonBarryChance { get; } =
        new("Button Barry Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<ButtonBarryOptions>.Instance.ButtonBarryAmount > 0
        };
        

    [ModdedNumberOption("Button Cooldown", 2.5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 30f;

    [ModdedNumberOption("Max Uses", 1f, 3f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxNumButtons { get; set; } = 1f;

    [ModdedToggleOption("Ignore Sabotages")]
    public bool IgnoreSabo { get; set; } = true;

    [ModdedToggleOption("Allow Usage in First Round")]
    public bool FirstRoundUse { get; set; } = false;
}