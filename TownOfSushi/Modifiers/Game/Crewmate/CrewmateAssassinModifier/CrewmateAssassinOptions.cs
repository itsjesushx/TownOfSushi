using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CrewmateAssassinOptions : AbstractOptionGroup<CrewmateAssassinModifier>
{
    public override string GroupName => "Crewmate Assassin";
    public override uint GroupPriority => 4;
    public override bool ShowInModifiersMenu => true;
    public override Color GroupColor => TownOfSushiColors.Crewmate;

    [ModdedNumberOption("Double Shot Amount", 0, 5)]
    public float DoubleShotAmount { get; set; } = 0;
    public ModdedNumberOption DoubleShotChance { get; } =
        new("Double Shot Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<CrewmateAssassinOptions>.Instance.DoubleShotAmount > 0
        };
}