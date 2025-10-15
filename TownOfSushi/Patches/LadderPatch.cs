// https://github.com/eDonnes124/Town-Of-Us-R/blob/ee0935bfbd35199b5d4f6f4ad9cf98621acb6d21/source/Patches/LadderFix.cs
using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(PlayerPhysics._CoClimbLadder_d__34), nameof(PlayerPhysics._CoClimbLadder_d__34.MoveNext))]
public static class LadderFix
{
    public static void Postfix(PlayerPhysics._CoClimbLadder_d__34 __instance)
    {
        // -1 state means last frame of the coroutine
        if (__instance.__1__state >= 0)
        {
            return;
        }

        var player = __instance.__4__this.myPlayer;

        if (!__instance.source.IsTop && player.HasModifier<GiantModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("Giant player on ladder detected, snapping position.");
            player.NetTransform.SnapTo(player.transform.position + new Vector3(0, 0.25f));
        }

        if (__instance.source.IsTop && player.HasModifier<MiniModifier>())
        {
            Logger<TownOfSushiPlugin>.Error("Mini player on ladder detected, snapping position.");
            player.NetTransform.SnapTo(player.transform.position + new Vector3(0, -0.25f));
        }
    }
}