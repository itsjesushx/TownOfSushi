using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class FrostyOptions : AbstractOptionGroup<FrostyModifier>
{
    public override string GroupName => "Frosty";
    public override uint GroupPriority => 35;
    public override Color GroupColor => TownOfSushiColors.Frosty;

    [ModdedNumberOption("Frosty Amount", 0, 5)]
    public float FrostyAmount { get; set; } = 0;

    public ModdedNumberOption FrostyChance { get; } =
        new("Frosty Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<FrostyOptions>.Instance.FrostyAmount > 0
        };

    [ModdedNumberOption("Chill Duration", 0f, 15f, suffixType: MiraNumberSuffixes.Seconds)]
    public float ChillDuration { get; set; } = 10f;

    [ModdedNumberOption("Chill Start Speed", 0.25f, 0.95f, 0.05f, MiraNumberSuffixes.Multiplier)]
    public float ChillStartSpeed { get; set; } = 0.75f;
}