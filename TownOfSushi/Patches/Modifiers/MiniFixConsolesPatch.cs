﻿using HarmonyLib;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;

namespace TownOfSushi.Patches.Modifiers;

[HarmonyPatch]
public static class MiniFixConsolesPatch
{
    // kinda scuffed, but UsableDistance is inlined so
    private static readonly Dictionary<Console, float> originalDistances = [];

    [HarmonyPrefix]
    [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
    public static void UsableDistancePatch(Console __instance)
    {
        var player = PlayerControl.LocalPlayer;
        if (!player || !player.HasModifier<MiniModifier>()) return;

        if (!originalDistances.TryGetValue(__instance, out var value))
        {
            value = __instance.usableDistance;
            originalDistances[__instance] = value;
        }

        __instance.usableDistance = value + 0.2f;
    }
}