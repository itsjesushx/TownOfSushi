using TMPro;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (ShowRoundOneShield.FirstRoundShielded != null && !ShowRoundOneShield.FirstRoundShielded.Data.Disconnected)
            {
                ShowRoundOneShield.FirstRoundShielded.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
                ShowRoundOneShield.FirstRoundShielded.myRend().material.SetFloat("_Outline", 0f);
                ShowRoundOneShield.FirstRoundShielded = null;
            }
        }
    }

    [HarmonyPatch]
    class MeetingHudPatch 
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.Select))]
        class PlayerVoteAreaSelectPatch 
        {
            static bool Prefix(MeetingHud __instance) 
            {
                return !(PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && Roles.Crewmates.Killing.VigilanteRole.AddButton.vigilanteUI != null) ||
                     !(PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && Roles.Neutral.Evil.DoomsayerRole.AddButton.doomsayerUI != null) ||
                     !(PlayerControl.LocalPlayer != null && PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && Roles.Abilities.AbilityMod.AssassinAbility.AddButton.assassinUI != null);
            }
        }

        static void populateButtonsPostfix(MeetingHud __instance) {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.IsJailed() && !PlayerControl.LocalPlayer.Data.IsDead && GetRole<Vigilante>(PlayerControl.LocalPlayer).RemainingKills > 0)
            {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (Roles.Crewmates.Killing.VigilanteRole.AddButton.IsExempt(playerVoteArea)) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "ShootButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = TownOfSushi.TargetIcon;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() => Roles.Crewmates.Killing.VigilanteRole.AddButton.vigilanteOnClick(copiedIndex, __instance)));
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer) && !PlayerControl.LocalPlayer.IsJailed() && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (Roles.Neutral.Evil.DoomsayerRole.AddButton.IsExempt(playerVoteArea)) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "ShootButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = TownOfSushi.TargetIcon;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() => Roles.Neutral.Evil.DoomsayerRole.AddButton.doomsayerOnClick(copiedIndex, __instance)));
                }
            }

            if (PlayerControl.LocalPlayer.Is(AbilityEnum.Assassin) && !PlayerControl.LocalPlayer.IsJailed() && !PlayerControl.LocalPlayer.Data.IsDead && Ability.GetAbility<Assassin>(PlayerControl.LocalPlayer).RemainingKills > 0)
            {
                for (int i = 0; i < __instance.playerStates.Length; i++)
                {
                    PlayerVoteArea playerVoteArea = __instance.playerStates[i];
                    if (Roles.Abilities.AbilityMod.AssassinAbility.AddButton.IsExempt(playerVoteArea)) continue;

                    GameObject template = playerVoteArea.Buttons.transform.Find("CancelButton").gameObject;
                    GameObject targetBox = UnityEngine.Object.Instantiate(template, playerVoteArea.transform);
                    targetBox.name = "ShootButton";
                    targetBox.transform.localPosition = new Vector3(-0.95f, 0.03f, -1.3f);
                    SpriteRenderer renderer = targetBox.GetComponent<SpriteRenderer>();
                    renderer.sprite = TownOfSushi.TargetIcon;
                    PassiveButton button = targetBox.GetComponent<PassiveButton>();
                    button.OnClick.RemoveAllListeners();
                    int copiedIndex = i;
                    button.OnClick.AddListener((System.Action)(() => Roles.Abilities.AbilityMod.AssassinAbility.AddButton.assassinOnClick(copiedIndex, __instance)));
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ServerStart))]
        class MeetingServerStartPatch {
            static void Postfix(MeetingHud __instance)
            {
                populateButtonsPostfix(__instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Deserialize))]
        class MeetingDeserializePatch {
            static void Postfix(MeetingHud __instance, [HarmonyArgument(0)]MessageReader reader, [HarmonyArgument(1)]bool initialState)
            {
                if (initialState) {
                    populateButtonsPostfix(__instance);
                }
            }
        }
    }
    [HarmonyPatch]
    public class ShowHostMeetingPatch
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        [HarmonyPostfix]

        public static void Postfix(MeetingHud __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

            var host = GameData.Instance.GetHost();

            if (host != null)
            {
                PlayerMaterial.SetColors(host.DefaultOutfit.ColorId, __instance.HostIcon);
                __instance.ProceedButton.gameObject.GetComponentInChildren<TextMeshPro>().text = $"host: {host.PlayerName}";
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        [HarmonyPostfix]

        public static void Setup(MeetingHud __instance)
        {
            if (AmongUsClient.Instance.NetworkMode != NetworkModes.OnlineGame) return;

            __instance.ProceedButton.gameObject.transform.localPosition = new(-2.5f, 2.2f, 0);
            __instance.ProceedButton.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            __instance.ProceedButton.GetComponent<PassiveButton>().enabled = false;
            __instance.HostIcon.enabled = true;
            __instance.HostIcon.gameObject.SetActive(true);
            __instance.ProceedButton.gameObject.SetActive(true);
        }
    }
}