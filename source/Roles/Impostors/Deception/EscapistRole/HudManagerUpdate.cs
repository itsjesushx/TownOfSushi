﻿namespace TownOfSushi.Roles.Impostors.Deception.EscapistRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite MarkSprite => TownOfSushi.MarkSprite;
        public static Sprite EscapeSprite => TownOfSushi.EscapeSprite;


        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Escapist)) return;
            var role = GetRole<Escapist>(PlayerControl.LocalPlayer);
            if (role.EscapeButton == null)
            {
                role.EscapeButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.EscapeButton.graphic.enabled = true;
                role.EscapeButton.graphic.sprite = MarkSprite;
                role.EscapeButton.gameObject.SetActive(false);

            }

            if (role.EscapeButton.graphic.sprite != MarkSprite && role.EscapeButton.graphic.sprite != EscapeSprite)
                role.EscapeButton.graphic.sprite = MarkSprite;

            role.EscapeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.EscapeButton.graphic.color = Palette.EnabledColor;
            role.EscapeButton.graphic.material.SetFloat("_Desat", 0f);
            if (role.EscapeButton.graphic.sprite == MarkSprite) role.EscapeButton.SetCoolDown(0f, 1f);
            else role.EscapeButton.SetCoolDown(role.EscapeTimer(), CustomGameOptions.EscapeCd);
        }
    }
}