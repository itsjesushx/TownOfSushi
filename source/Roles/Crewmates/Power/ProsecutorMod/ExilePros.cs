namespace TownOfSushi.Roles.Crewmates.Power.ProsecutorRole
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => ExilePros.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ExilePros
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Prosecutor))
            {
                var pros = (Prosecutor)role;
                if (pros.ProsecuteThisMeeting)
                {
                    var exiled = __instance.initData.networkedPlayer?.Object;
                    if (exiled != null)
                    {
                        pros.Player.Exiled();
                        pros.DeathReason = DeathReasonEnum.Suicide;
                    }
                    pros.ProsecuteThisMeeting = false;
                    pros.HasProsecuted = true;
                }
            }
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}