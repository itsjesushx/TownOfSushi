using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Impostor;
using UnityEngine;

namespace TownOfSushi.Options.Modifiers.Impostor;

public sealed class SaboteurOptions : AbstractOptionGroup<SaboteurModifier>
{
    public override string GroupName => "Saboteur";
    public override Color GroupColor => Palette.ImpostorRoleHeaderRed;
    public override uint GroupPriority => 41;

    [ModdedNumberOption("Reduced Sabotage Bonus", 5f, 15f, 1f, MiraNumberSuffixes.Seconds, "0")]
    public float ReducedSaboCooldown { get; set; } = 10f;
}