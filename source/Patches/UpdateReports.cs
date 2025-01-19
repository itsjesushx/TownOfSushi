namespace TownOfSushi.Patches
{
    //Disable report button when 2 players are left alive only
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ReportButtonUpdatePatch
    {
        static void UpdateReportButton(HudManager __instance) 
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return;
            if (MeetingHud.Instance  || PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2)
            {
                __instance.ReportButton.ToggleVisible(false);
            }
        }
        public static void Postfix(HudManager __instance)
        {
            UpdateReportButton(__instance);
        }
    }

    //so reports can't happen by clicking the corpse either
    [HarmonyPatch(typeof(DeadBody), nameof(DeadBody.OnClick))]
    public static class DeadBodyOnClickUpdate
    {
        public static bool Prefix(DeadBody __instance) 
        {
            if (GameOptionsManager.Instance.currentGameOptions.GameMode == GameModes.HideNSeek) return false;
            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2)  return false;
            return true;
        }
    }
}