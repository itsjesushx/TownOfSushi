namespace TownOfSushi.Roles.Crewmates.Support.GuardianMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudProtect
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Guardian)) return;
            var protectButton = __instance.KillButton;

            var role = GetRole<Guardian>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            protectButton.SetCoolDown(role.ProtectTimer(), CustomGameOptions.ConfessCd);

            var notProtected = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => x != role.Target)
                .ToList();

            SetTarget(ref role.ClosestPlayer, protectButton, float.NaN, notProtected);

            var renderer = protectButton.graphic;

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
