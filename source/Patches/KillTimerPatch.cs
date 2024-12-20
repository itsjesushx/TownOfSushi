namespace TownOfSushi
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class PatchKillTimer
    {
        public static bool GameStarted = false;
        [HarmonyPriority(Priority.First)]
        public static void Prefix(PlayerControl __instance, ref float time)
        {
            if (__instance.Data.IsImpostor() && time <= 11f
                && Math.Abs(__instance.killTimer - time) > 2 * Time.deltaTime
                && GameStarted == false)
            {
                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                    time = GameOptionsManager.Instance.currentHideNSeekGameOptions.KillCooldown - 0.25f;
                else time = CustomGameOptions.InitialCooldowns - 0.25f;
                GameStarted = true;
            }
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetKillTimer))]
    public static class KillTimer
    {
        public static bool Prefix(PlayerControl __instance, ref float time)
        {
            if (__instance.Data.Role.CanUseKillButton)
            {
                if (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown <= 0f)
                {
                    return false;
                }

                var maxvalue = time > GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown ? time + 1f : GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                __instance.killTimer = Mathf.Clamp(time, 0, maxvalue);
                DestroyableSingleton<HudManager>.Instance.KillButton.SetCoolDown(__instance.killTimer, maxvalue);
            }

            return false;
        } 
    }
}