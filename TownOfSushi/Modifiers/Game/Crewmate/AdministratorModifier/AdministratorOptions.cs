using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class AdministratorOptions : AbstractOptionGroup<AdministratorModifier>
{
    public override string GroupName => "Administrator";
    public override uint GroupPriority => 56;
    public override Color GroupColor => TownOfSushiColors.Administrator;
    
    [ModdedNumberOption("Administrator Amount", 0, 5)]
    public float AdministratorAmount { get; set; } = 0;

    [ModdedNumberOption("Administrator Chance", 0, 100, 10f, MiraNumberSuffixes.Percent)]
    public float AdministratorChance { get; set; } = 50f;

    [ModdedEnumOption("Who Sees Dead Bodies On Admin", typeof(AdminDeadPlayers),
        ["Nobody", "Administrator", "Everyone But Administrator", "Everyone"])]
    public AdminDeadPlayers WhoSeesDead { get; set; } = AdminDeadPlayers.Nobody;

    [ModdedToggleOption("Administrator Has Portable Admin")]
    public bool HasPortableAdmin { get; set; } = false;

    [ModdedToggleOption("Move While Using Portable Admin")]
    public bool MoveWithMenu { get; set; } = false;

    [ModdedNumberOption("Starting Charge", 20f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float StartingCharge { get; set; } = 20f;

    [ModdedNumberOption("Battery Charged Each Round", 15f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float RoundCharge { get; set; } = 15f;

    [ModdedNumberOption("Battery Charged Per Task", 10f, 30f, 2.5f, MiraNumberSuffixes.Seconds)]
    public float TaskCharge { get; set; } = 10f;

    [ModdedNumberOption("Portable Admin Display Cooldown", 15f, 30f, 5f, MiraNumberSuffixes.Seconds)]
    public float DisplayCooldown { get; set; } = 15f;

    [ModdedNumberOption("Portable Admin Display Duration", 15f, 30f, 5f, MiraNumberSuffixes.Seconds, zeroInfinity: true)]
    public float DisplayDuration { get; set; } = 15f;
}