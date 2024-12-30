﻿namespace TownOfSushi.Roles.Crewmates.Support.MedicRole
{
    [HarmonyPatch(typeof(HudManager))]
    public class HUDProtect
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Medic)) return;

            var protectButton = __instance.KillButton;
            var role = GetRole<Medic>(PlayerControl.LocalPlayer);

            protectButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            protectButton.SetCoolDown(role.StartTimer(), 10f);
            if (role.UsedAbility) return;
            SetTarget(ref role.ClosestPlayer, protectButton);
        }
    }
}