using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class AftermathOptions : AbstractOptionGroup<AftermathModifier>
{
    public override string GroupName => "Aftermath";
    public override uint GroupPriority => 38;
    public override Color GroupColor => TownOfSushiColors.Aftermath;

    [ModdedNumberOption("Aftermath Amount", 0, 5)]
    public float AftermathAmount { get; set; } = 0;

    public ModdedNumberOption AftermathChance { get; } =
        new("Aftermath Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<AftermathOptions>.Instance.AftermathAmount > 0
        };
}