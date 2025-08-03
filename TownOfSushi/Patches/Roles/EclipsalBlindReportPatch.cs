using HarmonyLib;
using InnerNet;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Neutral;

namespace TownOfSushi.Patches.Roles;

[HarmonyPatch]
public static class EclipsalBlindReportPatch
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPostfix]
    public static void HudManagerUpdatePatch(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null ||
            PlayerControl.LocalPlayer.Data == null ||
            !ShipStatus.Instance ||
            (AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Started && !TutorialManager.InstanceExists))
        {
            return;
        }

        if (PlayerControl.LocalPlayer.HasModifier<EclipsalBlindModifier>())
        {
            HudManager.Instance.ReportButton.SetActive(false);
        }
    }
}