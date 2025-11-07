using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class GeneralOptions : AbstractOptionGroup
{
    public override string GroupName => "General";
    public override uint GroupPriority => 1;

    [ModdedEnumOption("Modifier Type To Show In Role Intro", typeof(ModReveal))]
    public ModReveal ModifierReveal { get; set; } = ModReveal.Alliance;

    [ModdedToggleOption("Show Faction Modifier On Role Reveal")]
    public bool TeamModifierReveal { get; set; } = true;
    [ModdedNumberOption("Voting Time Added After Meeting Death", 0f, 15f, 1f, MiraNumberSuffixes.Seconds, "0.#")]
    public float AddedMeetingDeathTimer { get; set; } = 5f;

    [ModdedToggleOption("Camouflage Comms")]
    public bool CamouflageComms { get; set; } = true;

    public ModdedToggleOption KillDuringCamoComms { get; set; } = new("Kill Anyone During Camouflage", true)
    {
        Visible = () => OptionGroupSingleton<GeneralOptions>.Instance.CamouflageComms
    };

    public ModdedToggleOption CamoKillScreens { get; set; } = new("Camouflage Kill Screens During Comms", false)
    {
        Visible = () => OptionGroupSingleton<GeneralOptions>.Instance.CamouflageComms
    };

    [ModdedToggleOption("Impostors Don't Know Each Other")]
    public bool FFAImpostorMode { get; set; } = false;

    public ModdedToggleOption ImpsKnowRoles { get; set; } = new("Impostors Know Each Other's Roles", true)
    {
        Visible = () => !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode
    };

    public ModdedToggleOption ImpostorChat { get; set; } = new("Impostors Get A Private Meeting Chat", true)
    {
        Visible = () => !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode
    };

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
   /* [ModdedToggleOption("Disable Medbay Scan Walk Animation")]
    public bool DisableMedbayAnimation { get; set; } = false;*/

    [ModdedEnumOption("Disable Meeting Skip Button", typeof(SkipState))]
    public SkipState SkipButtonDisable { get; set; } = SkipState.No;

    [ModdedToggleOption("Shield Last Game First Kill")]
    public bool FirstDeathShield { get; set; } = true;

    [ModdedToggleOption("Powerful Crew Continue The Game")]
    public bool CrewKillersContinue { get; set; } = true;

    [ModdedToggleOption("Hide Vent Animations Not In Vision")]
    public bool HideVentAnimationNotInVision { get; set; } = true;
}

public enum ModReveal
{
    Alliance,
    Universal,
    Neither
}

public enum SkipState
{
    No,
    Emergency,
    Always
}