namespace TownOfSushi.Roles.Neutral.Killing.HitmanRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MorphUnMorph
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Hitman))
            {
                var hitman = (Hitman) role;
                if (hitman.IsUsingMorph)
                    Morph(hitman.Player, hitman.MorphTarget);
                else if (hitman.MorphTarget) Unmorph(hitman.Player);
            }
        }
    }
}