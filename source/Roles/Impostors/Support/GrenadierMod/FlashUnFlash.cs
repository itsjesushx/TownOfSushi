namespace TownOfSushi.Roles.Impostors.Support.GrenadierRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class FlashUnFlash
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Grenadier))
            {
                var grenadier = (Grenadier) role;
                if (grenadier.Flashed)
                    grenadier.Flash();
                else if (grenadier.Enabled) grenadier.UnFlash();
            }
        }
    }
}