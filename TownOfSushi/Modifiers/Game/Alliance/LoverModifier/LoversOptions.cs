using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Alliance;

public sealed class LoversOptions : AbstractOptionGroup<LoverModifier>
{
    public override string GroupName => "Lovers";
    public override uint GroupPriority => 11;
    public override Color GroupColor => TownOfSushiColors.Lover;
    
    [ModdedNumberOption("Lovers Chance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float LoversChance { get; set; } = 0;

    public ModdedToggleOption BothLoversDie { get; } = new("Both Lovers Die And Revive Together", true)
    {
        Visible = () => OptionGroupSingleton<LoversOptions>.Instance.LoversChance > 0
    };

    public ModdedNumberOption LovingImpPercent { get; } = new("Evil Lover Probability", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<LoversOptions>.Instance.LoversChance > 0
    };

    public ModdedToggleOption NeutralLovers { get; } = new("Neutral Roles Can Be Lovers", true)
    {
        Visible = () => OptionGroupSingleton<LoversOptions>.Instance.LoversChance > 0
    };

    public ModdedToggleOption LoverKillTeammates { get; } = new("Lover Can Kill Faction Teammates", true)
    {
        Visible = () => OptionGroupSingleton<LoversOptions>.Instance.LoversChance > 0
    };

    public ModdedToggleOption LoversKillEachOther { get; } = new("Lovers Can Kill One Another", true)
    {
        Visible = () => OptionGroupSingleton<LoversOptions>.Instance.LoversChance > 0
    };
}