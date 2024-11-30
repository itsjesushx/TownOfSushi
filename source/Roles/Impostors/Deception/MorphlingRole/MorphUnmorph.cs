namespace TownOfSushi.Roles.Impostors.Deception.MorphlingRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnmorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Morphling))
            {
                var morphling = (Morphling) role;
                if (morphling.Morphed)
                    morphling.MorphlingMorph();
                else if (MushroomSabotageActive()) morphling.MorphlingUnmorph();
                else if (morphling.MorphedPlayer) morphling.MorphlingUnmorph();
            }
        }
    }
}