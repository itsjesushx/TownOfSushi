﻿using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class VentPatches
{
    public static bool InVision(PlayerControl player)
    {
        var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
        Vector2 vector = player.GetTruePosition() - truePosition;
        var magnitude = vector.magnitude;

        if (magnitude < PlayerControl.LocalPlayer.lightSource.viewDistance &&
            !PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, magnitude, Constants.ShipAndObjectsMask))
        {
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
    [HarmonyPrefix]
    public static bool EnterVentPatch(Vent __instance, PlayerControl pc)
    {
        if (!__instance.EnterVentAnim) return true;
        if (!OptionGroupSingleton<GeneralOptions>.Instance.HideVentAnimationNotInVision) return true;

        if (InVision(pc))
        {
            return true;
        }

        return false;
    }

    [HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
    [HarmonyPrefix]
    public static bool ExitVentPatch(Vent __instance, PlayerControl pc)
    {
        if (!__instance.ExitVentAnim) return true;
        if (!OptionGroupSingleton<GeneralOptions>.Instance.HideVentAnimationNotInVision) return true;

        if (InVision(pc))
        {
            return true;
        }

        return false;
    }
}