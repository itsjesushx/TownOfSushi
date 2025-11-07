using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class CelebrityOptions : AbstractOptionGroup<CelebrityModifier>
{
    public override string GroupName => "Celebrity";
    public override uint GroupPriority => 39;
    public override Color GroupColor => TownOfSushiColors.Celebrity;

    [ModdedNumberOption("Celebrity Amount", 0, 5)]
    public float CelebrityAmount { get; set; } = 0;
    public ModdedNumberOption CelebrityChance { get; } =
        new("Celebrity Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<CelebrityOptions>.Instance.CelebrityAmount > 0
        };
}