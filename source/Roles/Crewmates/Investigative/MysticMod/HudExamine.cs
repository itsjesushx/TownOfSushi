﻿namespace TownOfSushi.Roles.Crewmates.Investigative.MysticMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudExamine
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateExamineButton(__instance);
        }

        public static void UpdateExamineButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;

            var examineButton = __instance.KillButton;
            var role = GetRole<Mystic>(PlayerControl.LocalPlayer);

            examineButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            examineButton.SetCoolDown(role.ExamineTimer(), CustomGameOptions.MysticExamineCd);
            SetTarget(ref role.ClosestPlayer, examineButton, float.NaN);

            var renderer = examineButton.graphic;
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