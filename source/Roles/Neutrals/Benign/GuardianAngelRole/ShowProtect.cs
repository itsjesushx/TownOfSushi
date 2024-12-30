namespace TownOfSushi.Roles.Neutral.Benign.GuardianAngelRole
{

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ShowProtect
    {
        public static Color ProtectedColor = new Color(1f, 0.85f, 0f, 1f);
        public static Color ShieldedColor = Color.cyan;

        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel) role;

                var player = ga.target;
                if (player == null) continue;

                if ((player.Data.IsDead || ga.Player.Data.IsDead || ga.Player.Data.Disconnected) && !player.IsShielded())
                {
                    player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                    player.myRend().material.SetFloat("_Outline", 0f);
                    continue;
                }
            }
        }
    }
}