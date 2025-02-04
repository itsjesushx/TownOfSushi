namespace TownOfSushi.Patches
{
    //Disable report button when 2 players are left alive only
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ReportButtonUpdatePatch
    {
        static void UpdateReportButton(HudManager __instance) 
        {
            if (IsHideNSeek()) return;
            if (Meeting()  || PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2)
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
            if (IsHideNSeek()) return false;
            if (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2)  return false;
            return true;
        }
    }

    //disable sabotage button when 2 players are left alive only or when the impostor is dead.
    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowSabotageMap))]
    public static class ShowSabotageMapPatch 
    {
        public static bool Prefix(MapBehaviour __instance) 
        {
            if (IsDead() || PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2)
            {
                __instance.ShowNormalMap();
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(SabotageButton), nameof(SabotageButton.Refresh))]
    class SabotageButtonFix
    {
        static void Postfix() 
        {
            if (IsDead() || PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count <= 2) 
            {
                HUDManager().SabotageButton.Hide();
            }
        }
    }
}