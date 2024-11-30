namespace TownOfSushi.Roles.Neutral.Killing.SerialKillerRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite StabSprite => TownOfSushi.StabSprite;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) return;
            var role = GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.StabKillCd);

            if (role.StabButton == null)
            {
                role.StabButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StabButton.graphic.enabled = true;
                role.StabButton.gameObject.SetActive(false);
            }

            role.StabButton.graphic.sprite = StabSprite;
            role.StabButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            role.StabButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stabbed)
            {
                role.StabButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.Stabeduration);
                role.StabButton.graphic.color = Palette.EnabledColor;
                role.StabButton.graphic.material.SetFloat("_Desat", 0f);
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

                return;
            }
            else
            {
                role.StabButton.SetCoolDown(role.StabTimer(), CustomGameOptions.StabCd);

                role.StabButton.graphic.color = Palette.EnabledColor;
                role.StabButton.graphic.material.SetFloat("_Desat", 0f);

                return;
            }
        }
    }
}
