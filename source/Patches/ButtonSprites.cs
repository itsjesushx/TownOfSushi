namespace TownOfSushi
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ButtonSpritesPatch
    {
        private static Sprite Kill;
        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.SeerSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.SeerSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.MedicSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.DouseSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.AlertSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.TrackSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.MediateSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.VultureEat;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Romantic))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.RomanticPick;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "PICK LOVER";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Crusader))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.FortifySprite;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "FORTIFY";
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.ProtectSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.InfectSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.EngineerFix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.TrapSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.ExamineSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.InspectSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.ObserveSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.JailSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.BiteSprite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = TownOfSushi.ConfessSprite;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "KILL";
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                GameOptionsManager.Instance.CurrentGameOptions.GameMode != GameModes.HideNSeek)
            {
                __instance.KillButton.transform.localPosition = new Vector3(0f, 1f, 0f);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer) || PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence)|| PlayerControl.LocalPlayer.Is(RoleEnum.Hitman) || PlayerControl.LocalPlayer.Is(RoleEnum.Vulture) ||PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)
                 || PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)  || PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner) || PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            }

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if (keyInt | controller && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }
                HudManager.Instance.AbilityButton.gameObject.SetActive(!MeetingHud.Instance);
            }
        }
    }
}
