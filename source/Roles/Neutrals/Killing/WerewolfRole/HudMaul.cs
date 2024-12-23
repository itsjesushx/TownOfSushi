namespace TownOfSushi.Roles.Neutral.Killing.WerewolfRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HUDMaul
    {
        public static Sprite Maul => TownOfSushi.MaulSprite;

        public static void Postfix(HudManager __instance)
        {
            if (NoButton(PlayerControl.LocalPlayer, RoleEnum.Werewolf))
                return;

            var role = GetRole<Werewolf>(PlayerControl.LocalPlayer);

            if (role.MaulButton == null)
            {
                role.MaulButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MaulButton.graphic.enabled = true;
                role.MaulButton.graphic.sprite = Maul;
                role.MaulButton.gameObject.SetActive(false);
            }

            role.MaulButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.MaulButton.SetCoolDown(role.MaulTimer(), CustomGameOptions.MaulCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.MaulButton);
            var renderer = role.MaulButton.graphic;
            
            if (role.ClosestPlayer != null && !role.MaulButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}