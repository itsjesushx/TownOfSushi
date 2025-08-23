using MiraAPI.GameOptions.Attributes;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class EscapistOptions : AbstractOptionGroup<EscapistRole>
{
    public override string GroupName => "Escapist";
    public override Color GroupColor => Palette.ImpostorRoleRed;

    [ModdedNumberOption("Recall Uses Per Game", 0f, 15f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxEscapes { get; set; } = 0f;

    [ModdedNumberOption("Recall Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float RecallCooldown { get; set; } = 25f;

    [ModdedToggleOption("Escapist Can Vent")]
    public bool CanVent { get; set; } = true;
}