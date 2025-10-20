using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Impostor;

public sealed class WarlockOptions : AbstractOptionGroup<WarlockRole>
{
    public override string GroupName => "Warlock";

    [ModdedNumberOption("Curse Cooldown", 1f, 30f, suffixType: MiraNumberSuffixes.Seconds)]
    public float CurseCooldown { get; set; } = 20f;

    [ModdedNumberOption("Root Time Duration", 5f, 10f, 1f, suffixType: MiraNumberSuffixes.Seconds)]
    public float RootTimeDuration { get; set; } = 7f;

    [ModdedToggleOption("Warlock Can Normally Kill With Teammate")]
    public bool WarlockKill { get; set; } = true;
}