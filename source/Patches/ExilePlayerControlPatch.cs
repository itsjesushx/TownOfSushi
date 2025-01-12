namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    [HarmonyPriority(Priority.First)]
    class ExileController2
    {
        public static NetworkedPlayerInfo lastExiled;
        public static void Prefix(ExileController __instance, [HarmonyArgument(0)]ref NetworkedPlayerInfo exiled)
        {
            lastExiled = exiled;

            var exiled2 = exiled.Object;
            var role = GetPlayerRole(exiled2);
            role.DeathReason = DeathReasonEnum.Ejected;

            var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
            foreach (var roles in witches)
            {
                foreach (var spelledId in roles.SpelledPlayers)
                {
                    var spelledPlayer = PlayerById(spelledId);
                    if (spelledPlayer != null && !spelledPlayer.Data.IsDead)
                    {
                        RpcMurderPlayer(spelledPlayer, spelledPlayer);
                        role.Kills++;

                        var role2 = GetPlayerRole(spelledPlayer);
                        role2.DeathReason = DeathReasonEnum.Cursed;
                        if (role2 != null)
                        {
                            role2.KilledBy = " By " + ColorString(Colors.Impostor, role.Player.name);            
                        }
                    }
                }
            }

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var seer = GetRole<Seer>(PlayerControl.LocalPlayer);
            seer.HasInvested1 = false;
            seer.HasInvested2 = false;
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