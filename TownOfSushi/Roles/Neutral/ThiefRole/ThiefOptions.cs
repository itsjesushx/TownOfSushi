using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class ThiefOptions : AbstractOptionGroup<ThiefRole>
{
    public override string GroupName => "Thief";

    [ModdedNumberOption("Thief Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedToggleOption("Thief Can Vent")]
    public bool CanVent { get; set; } = false;
    
    [ModdedToggleOption("Thief Has Impostor Vision")]
    public bool HasImpostorVision { get; set; } = false;
}