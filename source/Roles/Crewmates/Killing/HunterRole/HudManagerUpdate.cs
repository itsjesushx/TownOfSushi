namespace TownOfSushi.Roles.Crewmates.Killing.HunterRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite StalkSprite => TownOfSushi.StalkSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) return;

            var role = GetRole<Hunter>(PlayerControl.LocalPlayer);

                foreach (var player in role.CaughtPlayers)
                {
                    var data = player.Data;
                    if (data == null || data.Disconnected || data.IsDead || PlayerControl.LocalPlayer.Data.IsDead)
                        continue;

                    var colour = Color.black;
                    if (player.Is(AbilityEnum.Chameleon)) colour.a = GetAbility<Chameleon>(player).Opacity;
                    player.nameText().color = colour;
                }
            

            if (role.StalkButton == null)
            {
                role.StalkButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.StalkButton.graphic.enabled = true;
                role.StalkButton.gameObject.SetActive(false);
            }

            role.StalkButton.graphic.sprite = StalkSprite;
            role.StalkButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            if (role.UsesText == null && role.MaxUses > 0)
            {
                role.UsesText = Object.Instantiate(role.StalkButton.cooldownTimerText, role.StalkButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.MaxUses + "";
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) role.StalkButton.SetTarget(null);

            role.StalkButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Stalking) role.StalkButton.SetCoolDown(role.StalkDuration, CustomGameOptions.HunterStalkDuration);
            else if (role.StalkUsable) role.StalkButton.SetCoolDown(role.StalkTimer(), CustomGameOptions.HunterStalkCd);
            else role.StalkButton.SetCoolDown(0f, CustomGameOptions.HunterStalkCd);

            var renderer = role.StalkButton.graphic;
            if (role.Stalking || role.MaxUses == 0 || !PlayerControl.LocalPlayer.moveable) role.StalkButton.SetTarget(null);
            else
            {
                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) Utils.SetTarget(ref role.ClosestStalkPlayer, role.StalkButton, float.NaN);
                else Utils.SetTarget(ref role.ClosestStalkPlayer, role.StalkButton, float.NaN);
            }

            if (role.Stalking || (role.StalkUsable && role.ClosestStalkPlayer != null && PlayerControl.LocalPlayer.moveable))
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            __instance.KillButton.SetCoolDown(role.HunterKillTimer(), CustomGameOptions.HunterKillCd);
            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, role.CaughtPlayers);
            else Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, role.CaughtPlayers);

            return;
        }
    }
}