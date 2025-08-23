using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class UnderdogOptions : AbstractOptionGroup<UnderdogModifier>
{
    public override string GroupName => "Underdog";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 43;

    [ModdedNumberOption("Underdog Amount", 0, 5)]
    public float UnderdogAmount { get; set; } = 0;

    public ModdedNumberOption UnderdogChance { get; } =
        new("Underdog Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<UnderdogOptions>.Instance.UnderdogAmount > 0
        };
        
    [ModdedNumberOption("Kill Cooldown Bonus", 2.5f, 10f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldownIncrease { get; set; } = 5f;

    [ModdedToggleOption("Increased Kill Cooldown When 2+ Imps")]
    public bool ExtraImpsKillCooldown { get; set; } = false;
}