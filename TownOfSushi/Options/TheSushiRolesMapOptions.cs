using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class TownOfSushiMapOptions : AbstractOptionGroup
{
    public override string GroupName => "Map Options";
    public override uint GroupPriority => 6;

    [ModdedToggleOption("Enable Random Player Spawns")]
    public bool EnableRandomSpawns { get; set; } = false;

    [ModdedToggleOption("Enable Random Maps")]
    public bool RandomMaps { get; set; } = false;

    public ModdedNumberOption SkeldChance { get; } = new("Skeld Chance", 0, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
    };

    public ModdedNumberOption MiraChance { get; } = new("Mira Chance", 0, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
    };

    public ModdedNumberOption PolusChance { get; } = new("Polus Chance", 0, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
    };

    public ModdedNumberOption AirshipChance { get; } =
        new("Airship Chance", 0, 0, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
        };

    public ModdedNumberOption FungleChance { get; } = new("Fungle Chance", 0, 0, 100f, 10f, MiraNumberSuffixes.Percent)
    {
        Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
    };

    public ModdedNumberOption SubmergedChance { get; } =
        new("Submerged Chance", 0, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
        };

    public ModdedNumberOption LevelImpostorChance { get; } =
        new("Level Impostor Chance", 0, 0f, 100f, 10f, MiraNumberSuffixes.Percent)
        {
            Visible = () => OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps
        };
}