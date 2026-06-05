using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class SpeechlessOptions : AbstractOptionGroup<SpeechlessModifier>
{
    public override string GroupName => "Speechless";
    public override uint GroupPriority => 38;
    public override Color GroupColor => Color.gray;
    
    [ModdedNumberOption("Speechless Amount", 0, 5)]
    public float SpeechlessAmount { get; set; } = 0;

    public ModdedNumberOption SpeechlessChance { get; } = new("Speechless Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<SpeechlessOptions>.Instance.SpeechlessAmount > 0
    };
}