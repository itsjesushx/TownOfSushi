using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Options;

public sealed class NeutralRolesOptions : AbstractOptionGroup
{
    public override string GroupName => "Neutral Roles";
    public override Color GroupColor => TownOfSushiColors.Neutral;
    public override uint GroupPriority => 4;

    [ModdedNumberOption("Double Shot Amount", 0, 5)]
    public float DoubleShotAmount { get; set; } = 0;
    public ModdedNumberOption DoubleShotChance { get; } =
        new("Double Shot Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<NeutralRolesOptions>.Instance.DoubleShotAmount > 0
        };  
}