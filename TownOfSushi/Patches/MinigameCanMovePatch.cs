using HarmonyLib;
using MiraAPI.Hud;

namespace TownOfSushi.Patches;

[HarmonyPatch]
public static class MinigameCanMovePatch
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CanMove), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool PlayerControlCanMovePatch(PlayerControl __instance, ref bool __result)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return true;
        }

        if (MeetingHud.Instance)
        {
            return true;
        }

        if (PlayerControl.LocalPlayer.Data.Role is TransporterRole && ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard && OptionGroupSingleton<TransporterOptions>.Instance.MoveWithMenu && Minigame.Instance is CustomPlayerMenu)
        {
            __result = __instance.moveable;
            return false;
        }
        if (PlayerControl.LocalPlayer.Data.Role is GlitchRole && ActiveInputManager.currentControlType == ActiveInputManager.InputType.Keyboard && OptionGroupSingleton<GlitchOptions>.Instance.MoveWithMenu && Minigame.Instance is CustomPlayerMenu)
        {
            __result = __instance.moveable;
            return false;
        }

        return true;
    }
}