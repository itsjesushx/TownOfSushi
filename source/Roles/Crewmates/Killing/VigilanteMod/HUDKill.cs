namespace TownOfSushi.Roles.Crewmates.Killing.VigilanteRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDKill
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag7 = PlayerControl.AllPlayerControls.Count > 1;
            if (!flag7) return;
            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);
            if (flag8)
            {
                var role = GetRole<Vigilante>(PlayerControl.LocalPlayer);
                KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                KillButton.SetCoolDown(role.VigilanteKillTimer(), CustomGameOptions.VigilanteKillCd);

                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
            else
            {
                var isImpostor = PlayerControl.LocalPlayer.Data.IsImpostor();
                if (!isImpostor) return;

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;
                ImpKillTarget(KillButton);
            }
        }

        public static void ImpKillTarget(KillButton killButton)
        {
            PlayerControl target = null;

            if (!PlayerControl.LocalPlayer.moveable) target = null;
            else if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref target, killButton);
            else SetTarget(ref target, killButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Impostors)).ToList());
            killButton.SetTarget(target);
        }
    }
}