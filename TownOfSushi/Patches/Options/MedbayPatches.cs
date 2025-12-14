using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Patches.Options;

[HarmonyPatch(typeof(MedScanMinigame))]
public static class MedScanMinigameFixedUpdatePatch
{
    [HarmonyPatch(nameof(MedScanMinigame.FixedUpdate))]
    public static void Prefix(MedScanMinigame __instance)
    {
        if (OptionGroupSingleton<GeneralOptions>.Instance.ParallelMedbay)
        {
            // Allows multiple medbay scans at once
            __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
            __instance.medscan.UsersList.Clear();
        }
    }

    [HarmonyPatch(nameof(MedScanMinigame.Begin))]
    public static void Postfix(MedScanMinigame __instance)
    {
        if (PlayerControl.LocalPlayer.HasModifier<GiantModifier>())
        {
            __instance.completeString = __instance.completeString.Replace("3' 6\"", "5' 3\"").Replace("92lb", "184lb");
        }
        else if (PlayerControl.LocalPlayer.HasModifier<MiniModifier>())
        {
            __instance.completeString = __instance.completeString.Replace("3' 6\"", "1' 9\"").Replace("92lb", "46lb");
        }
    }

    [HarmonyPatch(typeof(MedScanMinigame._WalkToPad_d__16), nameof(MedScanMinigame._WalkToPad_d__16.MoveNext))]
    static class MedbayAnimationPatch_WalkToPad
    {
        [SuppressMessage("SonarAnalyzer", "S1144", Justification = "Used by Harmony")]
        [SuppressMessage("SonarAnalyzer", "S3218", Justification = "Harmony patch method, not shadowing")]
        static bool Prefix(MedScanMinigame._WalkToPad_d__16 __instance)
        {
            if (OptionGroupSingleton<GeneralOptions>.Instance.DisableMedbayAnimation)
            {
                var medScanMinigame = __instance.__4__this;
                switch (__instance.__1__state)
                {
                    case 0:
                        __instance.__1__state = -1;
                        medScanMinigame.state = MedScanMinigame.PositionState.WalkingToPad;
                        __instance.__1__state = 1;
                        return true;
                    case 1:
                        __instance.__1__state = -1;
                        __instance.__2__current = new WaitForSeconds(0.1f);
                        __instance.__1__state = 2;
                        return true;
                    case 2:
                        __instance.__1__state = -1;
                        medScanMinigame.walking = null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(MedScanMinigame._WalkToOffset_d__15), nameof(MedScanMinigame._WalkToOffset_d__15.MoveNext))]
    static class MedbayAnimationPatch_WalkToOffset
    {
        [SuppressMessage("SonarAnalyzer", "S1144", Justification = "Used by Harmony")]
        [SuppressMessage("SonarAnalyzer", "S3218", Justification = "Harmony patch method, not shadowing")]
        static bool Prefix(MedScanMinigame._WalkToOffset_d__15 __instance)
        {
            if (OptionGroupSingleton<GeneralOptions>.Instance.DisableMedbayAnimation)
            {
                var medScanMinigame = __instance.__4__this;
                switch (__instance.__1__state)
                {
                    case 0:
                        __instance.__1__state = -1;
                        medScanMinigame.state = MedScanMinigame.PositionState.WalkingToOffset;
                        __instance.__1__state = 1;
                        return true;
                    case 1:
                        __instance.__1__state = -1;
                        __instance.__2__current = new WaitForSeconds(0.1f);
                        __instance.__1__state = 2;
                        return true;
                    case 2:
                        __instance.__1__state = -1;
                        medScanMinigame.walking = null;
                        return false;
                    default:
                        return false;
                }
            }
            return false;
        }
    }
}