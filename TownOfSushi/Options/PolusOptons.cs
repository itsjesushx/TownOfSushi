using MiraAPI.GameOptions.Attributes;

namespace TownOfSushi.Options;

public sealed class BetterPolusOptions: AbstractOptionGroup
{
    public override string GroupName => "Better Polus";
    public override uint GroupPriority => 5;

    public override Func<bool> GroupVisible => () =>
        GameOptionsManager.Instance.currentGameOptions.MapId == (int)ShipStatus.MapType.Pb ||
        (OptionGroupSingleton<TownOfSushiMapOptions>.Instance.RandomMaps &&
         OptionGroupSingleton<TownOfSushiMapOptions>.Instance.PolusChance > 0);

    [ModdedToggleOption("Better Polus Vent Network")]
    public bool BPVentNetwork { get; set; } = false;

    [ModdedToggleOption("Vitals Moved To Lab")]
    public bool BPVitalsInLab { get; set; } = false;

    [ModdedToggleOption("Cold Temp Moved To Death Valley")]
    public bool BPTempInDeathValley { get; set; } = false;

    [ModdedToggleOption("Reboot Wifi And Chart Course Swapped")]
    public bool BPSwapWifiAndChart { get; set; } = false;

    [ModdedToggleOption("Add Custom Specimen Vent")]
    public bool BPCustomSpeciVent { get; set; } = false;
    [ModdedToggleOption("Polus Doors Are Airship Doors")]
    public bool AirshipDoors { get; set; } = false;
}