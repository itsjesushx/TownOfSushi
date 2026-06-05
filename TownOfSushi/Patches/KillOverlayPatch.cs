using System.Collections;
using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(OverlayKillAnimation), nameof(OverlayKillAnimation.CoShow), typeof(KillOverlay))]
public static class KillOverlayPatch
{
    public static void Prefix(OverlayKillAnimation __instance)
    {
        var flame = GameObject.Find("KillOverlay").transform.FindChild("QuadParent");
        if (flame != null)
        {
            flame.transform.localPosition = new Vector3(0f, 0f);
            if (flame.transform.FindChild("BackgroundFlame").TryGetComponent<SpriteRenderer>(out var flameSprite))
            {
                flameSprite.sprite = TownOfSushiAssets.KillBG.LoadAsset();
            }
        }

        if (ExileController.Instance)
        {
            if (flame != null)
            {
                flame.transform.localPosition = new Vector3(0, -1.5f);
                if (flame.transform.FindChild("BackgroundFlame").TryGetComponent<SpriteRenderer>(out var flameSprite))
                {
                    flameSprite.sprite = TownOfSushiAssets.RetributionBG.LoadAsset();
                }
            }

            __instance.GetComponentsInChildren<SpriteRenderer>(true).ToList()
                .ForEach(x => x.maskInteraction = SpriteMaskInteraction.None);
            __instance.transform.localPosition -= new Vector3(2.4f, 1.5f);
        }

        Coroutines.Start(CoKillDestroy());
    }
    public static void Postfix(OverlayKillAnimation __instance)
    {
        Coroutines.Start(CoKillDestroy());
    }

    private static IEnumerator CoKillDestroy()
    {
        // after 5 seconds, if a kill animation exists, then it will be destroyed
        var obj = GameObject.Find("KillOverlay").transform.GetChild(2).gameObject;
        yield return new WaitForSeconds(5f);

        obj?.Destroy();
    }
}