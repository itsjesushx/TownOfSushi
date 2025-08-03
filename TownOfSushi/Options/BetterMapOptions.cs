using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;

namespace TownOfSushi.Options;

public sealed class BetterMapOptions : AbstractOptionGroup
{
    public override string GroupName => "Better Polus";
    public override uint GroupPriority => 5;
    public override Func<bool> GroupVisible => () =>
        (GameOptionsManager.Instance.currentGameOptions.MapId == (int)ShipStatus.MapType.Pb) || (OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps && OptionGroupSingleton<TownOfSushiMapOptions>.Instance.PolusChance > 0);

    [ModdedToggleOption("Better Polus Vent Network")]
    public bool BPVentNetwork { get; set; } = false;

    [ModdedToggleOption("Polus: Vitals Moved To Lab")]
    public bool BPVitalsInLab { get; set; } = false;

    [ModdedToggleOption("Polus: Cold Temp Moved To Death Valley")]
    public bool BPTempInDeathValley { get; set; } = false;

    [ModdedToggleOption("Polus: Reboot Wifi And Chart Course Swapped")]
    public bool BPSwapWifiAndChart { get; set; } = false;
    
    [ModdedToggleOption("Airship: Airship Doors Are Polus Doors")]
    public bool AirshipPolusDoors { get; set; } = false;
}