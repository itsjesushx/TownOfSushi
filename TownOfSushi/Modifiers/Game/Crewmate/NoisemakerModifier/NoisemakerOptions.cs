using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class NoisemakerOptions : AbstractOptionGroup<NoisemakerModifier>
{
    public override string GroupName => "Noisemaker";
    public override uint GroupPriority => 34;
    public override Color GroupColor => TownOfSushiColors.Noisemaker;

    [ModdedNumberOption("Noisemaker Amount", 0, 5)]
    public float NoisemakerAmount { get; set; } = 0;

    public ModdedNumberOption NoisemakerChance { get; } =
        new("Noisemaker Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<NoisemakerOptions>.Instance.NoisemakerAmount > 0
        };
        

    [ModdedToggleOption("Impostors Get Alert")]
    public bool ImpostorsAlerted { get; set; } = true;

    [ModdedToggleOption("Neutral Killers Get Alert")]
    public bool NeutsAlerted { get; set; } = true;

    [ModdedToggleOption("Comms Sabotage Prevents Alert")]
    public bool CommsAffected { get; set; } = false;

    [ModdedToggleOption("Only Triggers If A Body Exists")]
    public bool BodyCheck { get; set; } = true;

    [ModdedNumberOption("Alert Duration", 1f, 20f, 1f, MiraNumberSuffixes.Seconds)]
    public float AlertDuration { get; set; } = 5f;
}