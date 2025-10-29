using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DecayOptions : AbstractOptionGroup<DecayModifier>
{
    public override string GroupName => "Decay";
    public override uint GroupPriority => 36;
    public override Color GroupColor => TownOfSushiColors.Decay;

    [ModdedNumberOption("Decay Amount", 0, 5)]
    public float DecayAmount { get; set; } = 0;
    
    public ModdedNumberOption DecayChance { get; } =
        new("Decay Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<DecayOptions>.Instance.DecayAmount > 0
        };

    [ModdedNumberOption("Time Before Body Rots Away", 0f, 25f, 1f, MiraNumberSuffixes.Seconds)]
    public float RotDelay { get; set; } = 5f;
}