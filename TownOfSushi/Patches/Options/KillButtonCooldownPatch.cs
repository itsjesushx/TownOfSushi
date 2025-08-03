using System.Globalization;
using HarmonyLib;

namespace TownOfSushi.Patches.Options;

[HarmonyPatch]
public static class KillButtonCooldownPatch
{
    [HarmonyPatch(typeof(ActionButton), nameof(ActionButton.SetCoolDown))]
    [HarmonyPostfix]
    public static void Postfix(ActionButton __instance, ref float timer)
    {
        if (__instance != HudManager.Instance.KillButton)
        {
            return;
        }

        if (!__instance.isActiveAndEnabled)
        {
            return;
        }

        if (!TownOfSushiPlugin.PreciseCooldowns.Value)
        {
            return;
        }

        if (__instance.isCoolingDown && timer <= 10f)
        {
            __instance.cooldownTimerText.text = timer.ToString("0.0", NumberFormatInfo.InvariantInfo);
        }
    }
}