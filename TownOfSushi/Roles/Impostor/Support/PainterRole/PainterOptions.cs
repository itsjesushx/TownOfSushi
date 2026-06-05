using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Impostor;

public sealed class PainterOptions : AbstractOptionGroup<PainterRole>
{
    public override string GroupName => "Painter";

    [ModdedNumberOption("Paint Uses Per Round", 0f, 10f, 1f, MiraNumberSuffixes.None, "0", true)]
    public float MaxPaints { get; set; } = 0f;

    [ModdedNumberOption("Paint Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PaintCooldown { get; set; } = 25f;

    [ModdedNumberOption("Paint Duration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float PaintDuration { get; set; } = 10f;

    [ModdedToggleOption("Paint Can Vent")]
    public bool PainterVent { get; set; } = true;
}