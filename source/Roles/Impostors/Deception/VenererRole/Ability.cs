namespace TownOfSushi.Roles.Impostors.Deception.VenererRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Ability
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Venerer))
            {
                var venerer = (Venerer) role;
                if (venerer.IsCamouflaged)
                    venerer.Ability();
                else if (MushroomSabotageActive()) venerer.StopAbility();
                else if (venerer.Enabled) venerer.StopAbility();
            }
        }
    }
}