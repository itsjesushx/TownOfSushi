﻿using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modifiers.Game.Crewmate;
using TownOfSushi.Options.Modifiers.Crewmate;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class MinigameCanMovePatch
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool PlayerControlCanMovePatch(PlayerControl __instance, ref bool __result)
    {
        if (PlayerControl.LocalPlayer == null) return true;
        if (MeetingHud.Instance) return true;
        // Only allows Scientist Vitals to allow you to move, not just vitals on the map
        if (PlayerControl.LocalPlayer.HasModifier<ScientistModifier>() && CustomButtonSingleton<ScientistButton>.Instance.EffectActive &&
            Minigame.Instance is VitalsMinigame && OptionGroupSingleton<ScientistOptions>.Instance.MoveWithMenu)
        {
            __result = __instance.moveable;
            return false;
        }
        if (PlayerControl.LocalPlayer.HasModifier<OperativeModifier>() && ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard && CustomButtonSingleton<SecurityButton>.Instance.EffectActive && CustomButtonSingleton<SecurityButton>.Instance.canMoveWithMinigame)
        {
            __result = __instance.moveable;
            return false;
        }

        return true;
    }
}