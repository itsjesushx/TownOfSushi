namespace TownOfSushi.Roles.Neutral.Benign.GuardianAngelRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class ProtectUnportect
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;
                if (ga.Protecting)
                    ga.Protect();
                else if (ga.Enabled) ga.UnProtect();
            }
        }
    }
}