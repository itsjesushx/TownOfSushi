using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Neutral;

public sealed class RomanticOptions : AbstractOptionGroup<RomanticRole>
{
    public override string GroupName => "Romantic";

    [ModdedNumberOption("Create Beloved Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float PickCooldown { get; set; } = 25f;

    [ModdedNumberOption("Protect Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float ProtectCooldown { get; set; } = 25f;

    [ModdedNumberOption("Protect Duration", 5f, 15f, 1f, MiraNumberSuffixes.Seconds)]
    public float ProtectDuration { get; set; } = 10f;

    [ModdedNumberOption("Max Number Of Protects", 1, 15, 1, MiraNumberSuffixes.None, "0")]
    public float MaxProtects { get; set; } = 5;

    [ModdedEnumOption("Show Protect To", typeof(RomanticProtectOptions), ["Romantic", "Self + Romantic", "Everyone"])]
    public RomanticProtectOptions ShowProtect { get; set; } = RomanticProtectOptions.SelfAndRomantic;

    [ModdedEnumOption("On Beloved Death, Romantic Becomes", typeof(BecomeOptions))]
    public BecomeOptions OnTargetDeath { get; set; } = BecomeOptions.Amnesiac;

    [ModdedToggleOption("Romantic Knows Beloved's Role")]
    public bool RomanticKnowsTargetRole { get; set; } = true;

    [ModdedToggleOption("Target Knows Romantic Exists")]
    public bool RomanticTargetKnows { get; set; } = true;
    

}

public enum RomanticProtectOptions
{
    Romantic,
    SelfAndRomantic,
    Everyone,
}