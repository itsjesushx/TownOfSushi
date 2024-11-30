namespace TownOfSushi.Roles.Impostors.Support.SwooperRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class SwoopUnswoop
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Swooper))
            {
                var swooper = (Swooper) role;
                if (swooper.IsSwooped)
                    swooper.Swoop();
                else if (MushroomSabotageActive()) swooper.UnSwoop();
                else if (swooper.Enabled) swooper.UnSwoop();
            }
        }
    }
}