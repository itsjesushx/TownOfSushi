using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Roles.Crewmate;

public sealed class VigilanteOptions : AbstractOptionGroup<VigilanteRole>
{
    public override string GroupName => "Vigilante";

    [ModdedNumberOption("Kill Cooldown", 10f, 60f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float KillCooldown { get; set; } = 25f;

    [ModdedToggleOption("Vigilante Can Self Report")]
    public bool VigilanteBodyReport { get; set; } = false;

    [ModdedToggleOption("Allow Shooting In First Round")]
    public bool FirstRoundUse { get; set; } = false;

    [ModdedToggleOption("Vigilante Can Shoot Neutral Evil Roles")]
    public bool ShootNeutralEvil { get; set; } = true;

    [ModdedEnumOption("Vigilante Misfire Kills", typeof(MisfireOptions), ["Self", "Target (Loses Ability)", "Self & Target", "No One"])]
    public MisfireOptions MisfireType { get; set; } = MisfireOptions.Vigilante;

    [ModdedNumberOption("Vigilante Number Of Guesses", 1f, 15f)]
    public float VigilanteKills { get; set; } = 5f;

    [ModdedToggleOption("Vigilante Can Guess More Than Once Per Meeting")]
    public bool VigilanteMultiKill { get; set; } = true;

    [ModdedToggleOption("Vigilante Can Guess Neutral Benign Roles")]
    public bool VigilanteGuessNeutralBenign { get; set; } = true;

    [ModdedToggleOption("Vigilante Can Guess Neutral Evil Roles")]
    public bool VigilanteGuessNeutralEvil { get; set; } = true;

    [ModdedToggleOption("Vigilante Can Guess Neutral Killing Roles")]
    public bool VigilanteGuessNeutralKilling { get; set; } = true;

    [ModdedToggleOption("Vigilante Can Guess Killer Modifiers")]
    public bool VigilanteGuessKillerMods { get; set; } = true;

    [ModdedToggleOption("Vigilante Can Guess Alliances")]
    public bool VigilanteGuessAlliances { get; set; } = true;

    [ModdedNumberOption("Vigilante Safe Shots Available", 0f, 3f, 1f, MiraNumberSuffixes.None, "0")]
    public float MultiShots { get; set; } = 3;
}

public enum MisfireOptions
{
    Vigilante,
    Target,
    Both,
    Nobody
}