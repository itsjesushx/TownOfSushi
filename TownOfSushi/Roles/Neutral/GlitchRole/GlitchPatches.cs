﻿using HarmonyLib;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles.Neutral;

[HarmonyPatch]
public static class GlitchPatches
{
    [HarmonyPatch(typeof(ReportButton), nameof(ReportButton.DoClick))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool DisabledReportButtonPatch(ActionButton __instance)
    {
        if (PlayerControl.LocalPlayer.HasModifier<DisabledModifier>() && !PlayerControl.LocalPlayer.GetModifier<DisabledModifier>()!.CanReport)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>())
        {
            PlayerControl.LocalPlayer.GetModifier<GlitchHackedModifier>()!.ShowHacked();
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(VentButton), nameof(VentButton.DoClick))]
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.DoClick))]
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.DoClick))]
    [HarmonyPriority(Priority.First)]
    [HarmonyPrefix]
    public static bool GlitchHackedSabotageButtonPatch(ActionButton __instance)
    {
        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>())
        {
            PlayerControl.LocalPlayer.GetModifier<GlitchHackedModifier>()!.ShowHacked();
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ToggleMapVisible))]
    [HarmonyPrefix]
    public static bool GlitchHackedToggleMapVisiblePatch(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() &&
            !PlayerControl.LocalPlayer.GetModifier<GlitchHackedModifier>()!.ShouldHideHacked)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<DisabledModifier>() && !PlayerControl.LocalPlayer.GetModifier<DisabledModifier>()!.CanUseAbilities)
        {
            return false;
        }

        return true;
    }
}