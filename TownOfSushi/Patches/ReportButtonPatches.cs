using AmongUs.GameOptions;
using HarmonyLib;
using InnerNet;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Patches;

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
        if (PlayerControl.LocalPlayer.HasModifier<SpeechlessModifier>())
        {
            HudManager.Instance.ReportButton.SetActive(false);
        }
    }
    
    // Reports that can't happen by clicking the corpse
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class DeadBodyOnClickUpdate
    {
        public static bool Prefix(DeadBody __instance) 
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return false;
            if (PlayerControl.LocalPlayer.HasModifier<EclipsalBlindModifier>())  return false;
            if (PlayerControl.LocalPlayer.HasModifier<SpeechlessModifier>())  return false;
            if (PlayerControl.LocalPlayer.HasModifier<DisabledModifier>() && !PlayerControl.LocalPlayer.GetModifier<DisabledModifier>()!.CanUseAbilities)  return false;
            return true;
        }
    }
}