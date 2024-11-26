namespace TownOfSushi.Roles.Neutral.Benign.RomanticRole
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudConfess
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Romantic)) return;
            var role = GetRole<Romantic>(PlayerControl.LocalPlayer);
            if (role.AlreadyPicked) return;
            
            var pickButton = __instance.KillButton;
            pickButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            pickButton.SetCoolDown(role.PickTimer(), CustomGameOptions.PickStartTimer);

            SetTarget(ref role.ClosestPlayer, pickButton, float.NaN);

            var renderer = pickButton.graphic;

            

            if (role.ClosestPlayer != null)
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