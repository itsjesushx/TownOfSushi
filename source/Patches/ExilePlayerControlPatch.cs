namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileController2
    {
        public static void Prefix()
        {
            var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
            foreach (var roles in witches)
            {
                foreach (var spelledId in roles.SpelledPlayers)
                {
                    var spelledPlayer = PlayerById(spelledId);
                    if (spelledPlayer != null && !spelledPlayer.Data.IsDead)
                    {
                        spelledPlayer.Exiled();
                        roles.Kills++;
                        GameHistory.CreateDeathReason(spelledPlayer, CustomDeathReason.WitchExile, roles.Player);
                    }
                }
            }

            var seer = AllRoles.Where(x => x.RoleType == RoleEnum.Seer  && !x.Player.Data.IsDead && x.Player != null).Cast<Seer>();
            foreach (var roles2 in seer)
            {
                roles2.HasInvested1 = false;
                roles2.HasInvested2 = false;
            }

            var deputy = AllRoles.Where(x => x.RoleType == RoleEnum.Deputy && !x.Player.Data.IsDead && x.Player != null).Cast<Deputy>();
            foreach (var roles3 in deputy)
            {
                roles3.HasExectutedAlready = false;
            }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    [HarmonyPriority(Priority.First)]
    class ExileControllerPatch
    {
        public static ExileController lastExiled;
        public static void Prefix(ExileController __instance)
        {
            lastExiled = __instance;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Exiled))]
    public static class ExilePlayerPatch
    {
        public static void Postfix(PlayerControl __instance)
        {
            PlayerHistory deadPlayer = new PlayerHistory(__instance, DateTime.UtcNow, CustomDeathReason.Exile, null);
            GameHistory.deadPlayers.Add(deadPlayer);
        }
    }

    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class ResetCustomTimersPatch
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
                ResetCustomTimers();
        }
    }
}