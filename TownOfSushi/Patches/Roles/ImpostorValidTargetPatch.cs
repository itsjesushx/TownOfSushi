﻿using HarmonyLib;
using MiraAPI.GameOptions;
using TownOfSushi.Options;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch(typeof(ImpostorRole), nameof(ImpostorRole.IsValidTarget))]
public static class ImpostorValidTargetPatch
{
    public static bool Prefix(ImpostorRole __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target,
        ref bool __result)
    {
        if (OptionGroupSingleton<GeneralOptions>.Instance.ImpsKnowRoles &&
            !OptionGroupSingleton<GeneralOptions>.Instance.FFAImpostorMode)
        {
            return true;
        }

        __result = target is { Disconnected: false, IsDead: false } &&
                   target.PlayerId != __instance.Player.PlayerId && target.Role && target.Object &&
                   !target.Object.inVent && !target.Object.inMovingPlat;
        return false;
    }
}