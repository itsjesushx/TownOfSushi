﻿using HarmonyLib;
using Reactor.Utilities;
using TownOfSushi.Modules;
using TownOfSushi.Roles;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(IntroCutscene._ShowRole_d__41), nameof(IntroCutscene._ShowRole_d__41.MoveNext))]
public static class SubmergedStartPatch
{
    public static void Postfix()
    {
        if (ModCompatibility.IsSubmerged())
        {
            Coroutines.Start(ModCompatibility.WaitMeeting(ModCompatibility.ResetTimers));
        }
    }
}

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class SubmergedHudPatch
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        if (ModCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.Data.IsDead &&
            PlayerControl.LocalPlayer.Data.Role is IGhostRole ghost)
        {
            __instance.MapButton.transform.parent.Find(__instance.MapButton.name + "(Clone)")?.gameObject
                ?.SetActive(!ghost.GhostActive);
        }
    }
}

[HarmonyPatch(typeof(PlayerPhysics))]
[HarmonyPriority(Priority.Low)] // make sure it occurs after other patches
public static class SubmergedLateUpdatePhysicsPatch
{
    [HarmonyPatch(nameof(PlayerPhysics.HandleAnimation))]
    [HarmonyPatch(nameof(PlayerPhysics.LateUpdate))]
    public static void Postfix(PlayerPhysics __instance)
    {
        if (!ModCompatibility.IsSubmerged())
        {
            return;
        }

        ModCompatibility.GhostRoleFix(__instance);
    }
}