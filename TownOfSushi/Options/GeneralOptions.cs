using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override uint GroupPriority => 1;
    
    [ModdedNumberOption("Voting Time Added After Meeting Death", 0f, 15f, 1f, MiraNumberSuffixes.Seconds, "0.#")]
    public float AddedMeetingDeathTimer { get; set; } = 5f;

    [ModdedToggleOption("Impostors Know Each Other's Roles")]
    public bool ImpsKnowRoles { get; set; } = true;
    
    [ModdedToggleOption("Impostors Get A Private Meeting Chat")]
    public bool ImpostorChat { get; set; } = true;

    [ModdedToggleOption("Vampires Get A Private Meeting Chat")]
    public bool VampireChat { get; set; } = true;

    [ModdedToggleOption("The Dead Know Everything")]
    public bool TheDeadKnow { get; set; } = true;

    [ModdedNumberOption("Game Start Cooldowns", 10f, 30f, 2.5f, MiraNumberSuffixes.Seconds, "0.#")]
    public float GameStartCd { get; set; } = 15f;

    [ModdedNumberOption("Temp Save Cooldown Reset", 0f, 30f, 0.5f, MiraNumberSuffixes.Seconds, "0.#")]
    public float TempSaveCdReset { get; set; } = 5f;

    [ModdedToggleOption("Parallel Medbay Scans")]
    public bool ParallelMedbay { get; set; } = true;
    [ModdedToggleOption("Disable Medbay Scan Walk Animation")]
    public bool DisableMedbayAnimation { get; set; } = false;

    [ModdedEnumOption("Disable Meeting Skip Button", typeof(SkipState))]
    public SkipState SkipButtonDisable { get; set; } = SkipState.No;

    [ModdedToggleOption("Shield Last Game First Kill")]
    public bool FirstDeathShield { get; set; } = true;

    [ModdedToggleOption("Powerful Crew Continue The Game")]
    public bool CrewKillersContinue { get; set; } = true;

    [ModdedToggleOption("Hide Vent Animations Not In Vision")]
    public bool HideVentAnimationNotInVision { get; set; } = true;
}

public enum SkipState
{
    No,
    Emergency,
    Always
}