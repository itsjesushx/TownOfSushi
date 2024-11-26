namespace TownOfSushi.Roles.Crewmates.Killing.VeteranMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class AlertUnalert
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Veteran))
            {
                var veteran = (Veteran) role;
                if (veteran.OnAlert)
                    veteran.Alert();
                else if (veteran.Enabled) veteran.UnAlert();
            }
        }
    }
}