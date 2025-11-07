using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SatelliteOptions : AbstractOptionGroup<SatelliteModifier>
{
    public override string GroupName => "Satellite";
    public override uint GroupPriority => 27;
    public override Color GroupColor => TownOfSushiColors.Satellite;

    [ModdedNumberOption("Satellite Amount", 0, 5)]
    public float SatelliteAmount { get; set; } = 0;

    public ModdedNumberOption SatelliteChance { get; } =
        new("Satellite Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<SatelliteOptions>.Instance.SatelliteAmount > 0
        };
        

    [ModdedNumberOption("Button Cooldown", 5f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float Cooldown { get; set; } = 15f;

    [ModdedNumberOption("Max Uses", 1f, 15f, 1f, MiraNumberSuffixes.None, "0")]
    public float MaxNumCast { get; set; } = 5f;

    [ModdedToggleOption("One Usage Per Round")]
    public bool OneUsePerRound { get; set; } = true;

    [ModdedToggleOption("Allow Usage in First Round")]
    public bool FirstRoundUse { get; set; } = true;
}