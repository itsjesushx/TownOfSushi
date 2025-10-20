using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class SaboteurOptions : AbstractOptionGroup<SaboteurModifier>
{
    public override string GroupName => "Saboteur";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 41;

    [ModdedNumberOption("Saboteur Amount", 0, 5)]
    public float SaboteurAmount { get; set; } = 0;

    public ModdedNumberOption SaboteurChance { get; } =
        new("Saboteur Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<SaboteurOptions>.Instance.SaboteurAmount > 0
        };
        

    [ModdedNumberOption("Reduced Sabotage Bonus", 5f, 15f, 1f, MiraNumberSuffixes.Seconds, "0")]
    public float ReducedSaboCooldown { get; set; } = 10f;
}