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
    public class KillButtonSprite
    {
        private static Sprite Fix =>  TownOfSushi.EngineerFix;
        private static Sprite Medic => TownOfSushi.MedicSprite;
        private static Sprite Seer => TownOfSushi.SeerSprite;
        private static Sprite Douse => TownOfSushi.DouseSprite;
        private static Sprite Jail => TownOfSushi.JailSprite;
        private static Sprite Alert => TownOfSushi.AlertSprite;
        private static Sprite Remember => TownOfSushi.RememberSprite;
        private static Sprite Track => TownOfSushi.TrackSprite;
        private static Sprite Transport => TownOfSushi.TransportSprite;
        private static Sprite Mediate => TownOfSushi.MediateSprite;
        private static Sprite Protect => TownOfSushi.ProtectSprite;
        private static Sprite RomanticPick => TownOfSushi.RomanticPick;
        private static Sprite Eat => TownOfSushi.VultureEat;
        private static Sprite Infect => TownOfSushi.InfectSprite;
        private static Sprite Trap => TownOfSushi.TrapSprite;
        private static Sprite Inspect => TownOfSushi.InspectSprite;
        private static Sprite Observe => TownOfSushi.ObserveSprite;
        private static Sprite Bite => TownOfSushi.BiteSprite;
        private static Sprite Confess => TownOfSushi.ConfessSprite;
        private static Sprite Kill;
        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                __instance.KillButton.graphic.sprite = Seer;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = Medic;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = Douse;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = Mediate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                __instance.KillButton.graphic.sprite = Eat;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Romantic))
            {
                __instance.KillButton.graphic.sprite = RomanticPick;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = Protect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = Infect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = Fix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = Trap;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator) || PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) )
            {
                __instance.KillButton.graphic.sprite = Inspect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                __instance.KillButton.graphic.sprite = Observe;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.KillButton.graphic.sprite = Bite;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                __instance.KillButton.graphic.sprite = Jail;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = Confess;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
                __instance.KillButton.buttonLabelText.text = "Kill";
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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
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
                var ghostRole = false;
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                {
                    var haunter = GetRole<Haunter>(PlayerControl.LocalPlayer);
                    if (!haunter.Caught) ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = GetRole<Phantom>(PlayerControl.LocalPlayer);
                    if (!phantom.Caught) ghostRole = true;
                }
                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole /*&& ShowDeadBodies*/ && !MeetingHud.Instance);
            }
        }
    }
}
