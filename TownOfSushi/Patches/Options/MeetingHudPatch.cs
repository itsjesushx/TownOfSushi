using HarmonyLib;
using MiraAPI.LocalSettings;
using UnityEngine;

namespace TownOfSushi.Patches.Options;

[HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.SetCosmetics))]
public static class PlayerStates
{
    public static void Postfix(PlayerVoteArea __instance)
    {
        if (LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.DisableNameplates.Value)
            __instance.Background.sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;

        if (LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.DisableLevelIndicators.Value)
        {
            __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().enabled = false;
            __instance.LevelNumberText.GetComponentInParent<SpriteRenderer>().gameObject.SetActive(false);
        }
    }
}