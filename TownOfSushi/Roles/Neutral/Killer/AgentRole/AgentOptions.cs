using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Roles.Neutral;

public sealed class AgentOptions : AbstractOptionGroup<AgentRole>
{
    public override string GroupName => "Agent";

    [ModdedToggleOption("Agent/Hitman Can Use Vents")]
    public bool CanUseVents { get; set; } = true;

    [ModdedToggleOption("Agent Has Impostor Vision")]
    public bool HasImpostorVision { get; set; } = true;
    [ModdedNumberOption("Hitman Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedNumberOption("Hitman Morph Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float MorphCooldown { get; set; } = 25f;

    [ModdedNumberOption("Mimic Duration", 5f, 15f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float MorphDuration { get; set; } = 10f;

    [ModdedNumberOption("Hitman Drag Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float DragCooldown { get; set; } = 25f;

    [ModdedNumberOption("Hitman Drag Speed", 0.25f, 1f, 0.05f, MiraNumberSuffixes.Multiplier, "0.00")]
    public float DragSpeedMultiplier { get; set; } = 0.75f;

    [ModdedToggleOption("Dragging Speed Is Affected by Body Size")]
    public bool AffectedSpeed { get; set; } = true;
    public ModdedToggleOption CanVentWithBody { get; } = new ModdedToggleOption("Hitman Can Vent With Body", false)
    {
        Visible = () => OptionGroupSingleton<AgentOptions>.Instance.CanUseVents,
    };
}