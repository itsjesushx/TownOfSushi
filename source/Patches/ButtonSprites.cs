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
            __instance.KillButton.buttonLabelText.gameObject.SetActive(true);
            __instance.KillButton.OverrideText("");

            var flag = false;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.SampleButton.png", 115f);
                __instance.KillButton.OverrideText("Investigate");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.SampleButton.png", 115f);
                __instance.KillButton.OverrideText("Investigate");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.ShieldButton.png", 115f);
                __instance.KillButton.OverrideText("Shield");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Douse.png", 115f);
                __instance.KillButton.OverrideText("Douse");
                __instance.KillButton.transform.localPosition = ButtonPositions.lowerRowLeft;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.VeteranButton.png", 115f);
                __instance.KillButton.OverrideText("Alert");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Track.png", 115f);
                __instance.KillButton.OverrideText("TRACK");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Mediate.png", 115f);
                __instance.KillButton.OverrideText("Mediate");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vulture))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.VultureButton.png", 115f);
                __instance.KillButton.OverrideText("EAT");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Romantic))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Romantic.png", 115f);
                __instance.KillButton.OverrideText("Create Lover");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Crusader))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Fortify.png", 115f);
                __instance.KillButton.OverrideText("Fortify");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Protect.png", 115f);
                __instance.KillButton.OverrideText("PROTECT");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Infect.png", 115f);
                __instance.KillButton.OverrideText("Infect");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.RepairButton.png", 115f);
                __instance.KillButton.OverrideText("FIX");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Watch.png", 115f);
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Trapper_Place_Button.png", 115f);
                __instance.KillButton.OverrideText("TRAP");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.RevealButton.png", 115f);
                __instance.KillButton.OverrideText("REVEAL");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.SampleButton.png", 115f);
                __instance.KillButton.OverrideText("Investigate");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Observe.png", 115f);
                __instance.KillButton.OverrideText("Observe");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.JailButton.png", 115f);
                __instance.KillButton.OverrideText("JAIL");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Bite.png", 115f);
                __instance.KillButton.OverrideText("BITE");
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                __instance.KillButton.graphic.sprite = LoadSpriteFromResources("TownOfSushi.Resources.Confess.png", 115f);
                __instance.KillButton.OverrideText("Confess");
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.OverrideText("KILL");
                flag = PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) ||
                    PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller) || PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut);
            }
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors) &&
                !IsHideNSeek())
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
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, -0.06f, 0);
            }

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if (keyInt | controller && __instance.KillButton != null && flag && !IsDead())
                __instance.KillButton.DoClick();
            
            var role = GetPlayerRole(PlayerControl.LocalPlayer);
            bool AbilityKey = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToS imp/nk");
            if (role?.ExtraButtons != null && AbilityKey && !IsDead())
                role?.ExtraButtons[0]?.DoClick();

            if (GetAbility<ButtonBarry>(PlayerControl.LocalPlayer)?.ButtonUsed == false &&
                Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToS bb/disperse/mimic") &&
                !IsDead())
            {
                GetAbility<ButtonBarry>(PlayerControl.LocalPlayer).ButtonButton.DoClick();
            }
            else if (GetModifier<Disperser>(PlayerControl.LocalPlayer)?.MaxUses > 0 &&
                     Rewired.ReInput.players.GetPlayer(0).GetButtonDown("ToS bb/disperse/mimic/hitman") &&
                     !IsDead())
            {
                GetModifier<Disperser>(PlayerControl.LocalPlayer).DisperseButton.DoClick();
            }
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    HUDManager().AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (IsHideNSeek())
                {
                    HUDManager().AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }
                HUDManager().AbilityButton.gameObject.SetActive(!Meeting());
            }
        }
    }
}
