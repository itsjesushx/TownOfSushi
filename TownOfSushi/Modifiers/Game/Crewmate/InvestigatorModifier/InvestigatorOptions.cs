using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class InvestigatorOptions : AbstractOptionGroup<InvestigatorModifier>
{
    public override string GroupName => "Investigator";
    public override uint GroupPriority => 57;
    public override Color GroupColor => TownOfSushiColors.Investigator;

    [ModdedNumberOption("Investigator Amount", 0, 5)]
    public float InvestigatorAmount { get; set; } = 0;

    public ModdedNumberOption InvestigatorChance { get; } =
        new("Investigator Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<InvestigatorOptions>.Instance.InvestigatorAmount > 0
        };

    [ModdedNumberOption("Footprint Size", 1f, 10f, suffixType: MiraNumberSuffixes.Multiplier)]
    public float FootprintSize { get; set; } = 4f;

    [ModdedNumberOption("Footprint Interval", 0.5f, 6f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float FootprintInterval { get; set; } = 1;

    [ModdedNumberOption("Footprint Duration", 1f, 15f, 0.5f, MiraNumberSuffixes.Seconds)]
    public float FootprintDuration { get; set; } = 10f;

    [ModdedToggleOption("Anonymous Footprint")]
    public bool ShowAnonymousFootprints { get; set; } = false;

    [ModdedToggleOption("Footprint Vent Visible")]
    public bool ShowFootprintVent { get; set; } = false;
}