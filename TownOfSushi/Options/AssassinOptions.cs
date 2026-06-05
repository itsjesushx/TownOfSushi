using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class AssassinOptions : AbstractOptionGroup
{
    public override string GroupName => "Assassin Options";
    public override uint GroupPriority => 7;

    [ModdedNumberOption("Number Of Impostor Assassins", 0, 4, 1, MiraNumberSuffixes.None, "0")]
    public float NumberOfImpostorAssassins { get; set; } = 1;

    public ModdedNumberOption ImpAssassinChance { get; } =
        new("Impostor Assassin Chance", 100f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<AssassinOptions>.Instance.NumberOfImpostorAssassins > 0
        };

    [ModdedNumberOption("Number Of Neutral Assassins", 0, 5, 1, MiraNumberSuffixes.None, "0")]
    public float NumberOfNeutralAssassins { get; set; } = 1;

    [ModdedNumberOption("Number Of Crewmate Assassins", 0, 5, 1, MiraNumberSuffixes.None, "0")]
    public float NumberOfCrewmateAssassins { get; set; } = 1;

    public ModdedNumberOption NeutAssassinChance { get; } =
        new("Neutral Assassin Chance", 100f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<AssassinOptions>.Instance.NumberOfNeutralAssassins > 0
        };
    
    public ModdedNumberOption CrewAssassinChance { get; } =
        new("Crewmate Assassin Chance", 100f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<AssassinOptions>.Instance.NumberOfCrewmateAssassins > 0
        };


    public ModdedToggleOption AmneTurnImpAssassin { get; } = new($"Amnesiac Turned Impostor Gets Assassin Ability", true);
    public ModdedToggleOption ThiefTurnAssassin { get; } = new($"Thief Gets Assassin Ability", true);
    public ModdedToggleOption ThiefCrewTurnAssassin { get; } = new($"Thief Gets Assassin Ability If Sheriff Was Stolen", true);

    public ModdedToggleOption AmneTurnNeutAssassin { get; } = new($"Amnesiac Turned Neutral Killing Gets Assassin Ability", true);
    public ModdedToggleOption AmneTurnCrewAssassin { get; } = new($"Amnesiac Turned Crewmate Gets Assassin Ability", true);
    public ModdedToggleOption CantGuessProtectedPlayer { get; } = new($"Assassin Can't Guess Protected Players", true);

    [ModdedNumberOption("Number Of Assassin Kills", 1, 15, 1, MiraNumberSuffixes.None, "0")]
    public float AssassinKills { get; set; } = 5;

    [ModdedToggleOption("Assassin Can Kill More Than Once Per Meeting")]
    public bool AssassinMultiKill { get; set; } = true;

    [ModdedToggleOption("Assassin Can Guess \"Crewmate\"")]
    public bool AssassinCrewmateGuess { get; set; } = false;

    [ModdedToggleOption("Assassin Can Guess Crew Investigative Roles")]
    public bool AssassinGuessInvest { get; set; } = false;

    [ModdedToggleOption("Assassin Can Guess Neutral Benign Roles")]
    public bool AssassinGuessNeutralBenign { get; set; } = true;

    [ModdedToggleOption("Assassin Can Guess Neutral Evil Roles")]
    public bool AssassinGuessNeutralEvil { get; set; } = true;

    [ModdedToggleOption("Assassin Can Guess Crewmate Modifiers")]
    public bool AssassinGuessCrewModifiers { get; set; } = true;


    [ModdedToggleOption("Assassin Can Guess Killer Modifiers")]
    public bool AssassinGuessKillerMods { get; set; } = true;

    public ModdedToggleOption AssassinGuessUtilityModifiers { get; } =
        new("Assassin Can Guess Crew Utility Modifiers", false)
        {
            Visible = () => OptionGroupSingleton<AssassinOptions>.Instance.AssassinGuessCrewModifiers
        };

    [ModdedToggleOption("Assassin Can Guess Alliances")]
    public bool AssassinGuessAlliances { get; set; } = true;
}