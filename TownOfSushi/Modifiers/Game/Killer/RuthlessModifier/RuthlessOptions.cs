using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Killer;

public sealed class RuthlessOptions : AbstractOptionGroup<RuthlessModifier>
{
    public override string GroupName => "Ruthless";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 41;

    [ModdedNumberOption("Ruthless Amount", 0, 5)]
    public float RuthlessAmount { get; set; } = 0;

    public ModdedNumberOption RuthlessChance { get; } =
        new("Ruthless Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<RuthlessOptions>.Instance.RuthlessAmount > 0
        };
}