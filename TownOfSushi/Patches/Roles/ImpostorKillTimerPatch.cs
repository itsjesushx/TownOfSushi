﻿using HarmonyLib;
using UnityEngine;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch]
public static class ImpostorKillTimerPatch
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    [HarmonyPrefix]
    public static bool SetKillTimerPatch(PlayerControl __instance, ref float time)
    {
        if (__instance.Data.Role.CanUseKillButton)
        {
            if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f)
            {
                return false;
            }

            var maxvalue = time > GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown ? time + 1f : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
            __instance.killTimer = Mathf.Clamp(time, 0, maxvalue);
            HudManager.Instance.KillButton.SetCoolDown(__instance.killTimer, maxvalue);
        }

        return false;
    }
}