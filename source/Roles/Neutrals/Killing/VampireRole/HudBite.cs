namespace TownOfSushi.NeutralRoles.VampireRole
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudBite
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) return;
            var biteButton = __instance.KillButton;

            var role = GetRole<Vampire>(PlayerControl.LocalPlayer);

            biteButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            biteButton.SetCoolDown(role.BiteTimer(), CustomGameOptions.BiteCd);

            var notVampire = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !x.Is(RoleEnum.Vampire))
                .ToList();

            if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, biteButton);
            else SetTarget(ref role.ClosestPlayer, biteButton, float.NaN, notVampire);
        }
    }
}
