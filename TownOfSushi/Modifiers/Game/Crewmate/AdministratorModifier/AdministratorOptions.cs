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

    public ModdedNumberOption AdministratorChance { get; } =
        new("Administrator Chance", 50f, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.AdministratorAmount > 0
        };

    [ModdedEnumOption("Who Sees Dead Bodies On Admin", typeof(AdminDeadPlayers),
        ["Nobody", "Administrator", "Everyone But Administrator", "Everyone"])]
    public AdminDeadPlayers WhoSeesDead { get; set; } = AdminDeadPlayers.Nobody;

    [ModdedEnumOption("Allow Portable Admin Table For", typeof(PortableAdmin),
        ["Role", "Modifier", "Role & Modifier", "Disabled"])]
    public PortableAdmin HasPortableAdmin { get; set; } = PortableAdmin.Both;

    public ModdedToggleOption MoveWithMenu { get; } = new("Move While Using Portable Admin", true)
    {
        Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
    };

    public ModdedNumberOption StartingCharge { get; } =
        new("Starting Charge", 20f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
        {
            Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
        };

    public ModdedNumberOption RoundCharge { get; } =
        new("Battery Charged Each Round", 15f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
        {
            Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
        };

    public ModdedNumberOption TaskCharge { get; } =
        new("Battery Charged Per Task", 10f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
        {
            Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
        };

    public ModdedNumberOption DisplayCooldown { get; } = new("Portable Admin Display Cooldown", 15f, 0f, 30f, 5f,
        MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
    };

    public ModdedNumberOption DisplayDuration { get; } = new("Portable Admin Display Duration", 15f, 0f, 30f, 5f,
        MiraNumberSuffixes.Seconds, zeroInfinity: true)
    {
        Visible = () => OptionGroupSingleton<AdministratorOptions>.Instance.HasPortableAdmin is not PortableAdmin.None
    };
}

public enum PortableAdmin
{
    Role,
    Modifier,
    Both,
    None
}

public enum AdminDeadPlayers
{
    Nobody,
    Administrator,
    EveryoneButAdministrator,
    Everyone
}