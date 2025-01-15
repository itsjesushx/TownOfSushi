namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    public static class ExileRolesUpdate
    {
        public static void Prefix()
        {
            var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
            foreach (var role in witches)
            {
                foreach (var spelledId in role.SpelledPlayers)
                {
                    var spelledPlayer = PlayerById(spelledId);
                    if (spelledPlayer != null && !spelledPlayer.Data.IsDead)
                    {
                        spelledPlayer.Exiled();
                        role.Kills++;
                        GameHistory.CreateDeathReason(spelledPlayer, CustomDeathReason.WitchExile, role.Player);
                    }
                }
            }

            var seer = AllRoles.Where(x => x.RoleType == RoleEnum.Seer  && !x.Player.Data.IsDead && x.Player != null).Cast<Seer>();
            foreach (var role2 in seer)
            {
                role2.HasInvested1 = false;
                role2.HasInvested2 = false;
            }

            var deputy = AllRoles.Where(x => x.RoleType == RoleEnum.Deputy && !x.Player.Data.IsDead && x.Player != null).Cast<Deputy>();
            foreach (var role3 in deputy)
            {
                role3.HasExectutedAlready = false;
            }

            var crusader = AllRoles.Where(x => x.RoleType == RoleEnum.Crusader && !x.Player.Data.IsDead && x.Player != null).Cast<Crusader>();
            foreach (var role4 in crusader)
            {
                role4.Fortified = null;
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