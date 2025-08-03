using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;

namespace  TownOfSushi.Options.Modifiers.Crewmate;

public sealed class SpyOptions : AbstractOptionGroup<SpyModifier>
{
    public override string GroupName => "Spy";

    [ModdedEnumOption("Who Sees Dead Bodies On Admin", typeof(AdminDeadPlayers), ["Nobody", "Spy", "Everyone But Spy", "Everyone"])]
    public AdminDeadPlayers WhoSeesDead { get; set; } = AdminDeadPlayers.Nobody;

    [ModdedToggleOption("Allow Portable Admin Table")]
    public bool HasPortableAdmin { get; set; } = true;

    public ModdedToggleOption MoveWithMenu { get; } = new ModdedToggleOption("Move While Using Portable Admin", true)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };
    public ModdedNumberOption StartingCharge { get; } = new ModdedNumberOption("Starting Charge", 20f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };

    public ModdedNumberOption RoundCharge { get; } = new ModdedNumberOption("Battery Charged Each Round", 15f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };

    public ModdedNumberOption TaskCharge { get; } = new ModdedNumberOption("Battery Charged Per Task", 10f, 0f, 30f, 2.5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };

    public ModdedNumberOption DisplayCooldown { get; } = new ModdedNumberOption("Portable Admin Display Cooldown", 15f, 0f, 30f, 5f, MiraNumberSuffixes.Seconds)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };

    public ModdedNumberOption DisplayDuration { get; } = new ModdedNumberOption("Portable Admin Display Duration", 15f, 0f, 30f, 5f, MiraNumberSuffixes.Seconds, zeroInfinity: true)
    {
        Visible = () => OptionGroupSingleton<SpyOptions>.Instance.HasPortableAdmin,
    };
}

public enum PortableAdmin
{
    Role,
    Modifier,
    Both,
    None,
}


public enum AdminDeadPlayers
{
    Nobody,
    Spy,
    EveryoneButSpy,
    Everyone,
}
