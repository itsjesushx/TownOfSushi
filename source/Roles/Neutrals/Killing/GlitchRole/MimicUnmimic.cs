namespace TownOfSushi.Roles.Neutral.Killing.GlitchRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class MimicUnmimic
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch) role;
                if (glitch.IsUsingMimic)
                    Morph(glitch.Player, glitch.MimicTarget);
                else if (glitch.MimicTarget) Unmorph(glitch.Player);
            }
        }
    }
}