using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class BaitOptions : AbstractOptionGroup<BaitModifier>
{
    public override string GroupName => "Bait";
    public override uint GroupPriority => 32;
    public override Color GroupColor => TownOfSushiColors.Bait;
    
    [ModdedNumberOption("Bait Amount", 0, 5)]
    public float BaitAmount { get; set; } = 0;

    public ModdedNumberOption BaitChance { get; } = new("Bait Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<BaitOptions>.Instance.BaitAmount > 0
    };

    [ModdedNumberOption("Min Bait Report Delay", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float MinDelay { get; set; } = 0f;

    [ModdedNumberOption("Max Bait Report Delay", 0f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float MaxDelay { get; set; } = 1f;
}