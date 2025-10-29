using HarmonyLib;
using TownOfSushi.Options;

namespace TownOfSushi.Patches.PrefabSwitching;

[HarmonyPatch]
public static class AirshipDoors
{
    [HarmonyPatch(typeof(AirshipStatus), nameof(AirshipStatus.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix(AirshipStatus __instance)
    {
        if (!OptionGroupSingleton<AirshipOptions>.Instance.AirshipPolusDoors)
        {
            return;
        }

        var polusdoor = PrefabLoader.Polus.GetComponentInChildren<DoorConsole>().MinigamePrefab;
        foreach (var door in __instance.GetComponentsInChildren<DoorConsole>())
        {
            door.MinigamePrefab = polusdoor;
        }
    }
}

[HarmonyPatch]
public static class PolusDoors
{
    [HarmonyPatch(typeof(PolusShipStatus), nameof(PolusShipStatus.OnEnable))]
    [HarmonyPostfix]
    public static void Postfix(PolusShipStatus __instance)
    {
        if (!OptionGroupSingleton<BetterPolusOptions>.Instance.AirshipDoors)
        {
            return;
        }

        var airshipDoors = PrefabLoader.Airship.GetComponentInChildren<DoorConsole>().MinigamePrefab;
        foreach (var door in __instance.GetComponentsInChildren<DoorConsole>())
        {
            door.MinigamePrefab = airshipDoors;
        }
    }
}