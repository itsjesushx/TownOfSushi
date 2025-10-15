using HarmonyLib;
using MiraAPI.LocalSettings;
using TMPro;
using UnityEngine;

namespace TownOfSushi.Patches;

// Original patch taken from https://github.com/xChipseq/VanillaEnhancements/blob/main/VanillaEnhancements/Patches/DarkModePatches.cs

[HarmonyPatch()]
public static class DarkModePatches
{
    [HarmonyPatch(typeof(ChatBubble), nameof(ChatBubble.SetCosmetics))]
    public static class ChatBubblePatch
    {
        public static void Postfix(ChatBubble __instance)
        {
            if (!LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.EnableDarkMode.Value)
            {
                return;
            }

            __instance.Background.color = new Color(0.2f, 0.2f, 0.2f);
            __instance.MaskArea.color = new Color(0.1f, 0.1f, 0.1f);
            __instance.TextArea.color = new Color(1, 1, 1);
            __instance.TextArea.outlineWidth = __instance.NameText.outlineWidth * 0.75f;
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Awake))]
    public static class ChatController_Awake
    {
        public static void Postfix(ChatController __instance)
        {
            __instance.quickChatButton.gameObject.SetActive(false);
            __instance.openKeyboardButton.gameObject.SetActive(false);
        }
    }

    [HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
    public static class ChatControllerPatch
    {
        public static void Postfix(ChatController __instance)
        {
            if (!LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.EnableDarkMode.Value)
            {
                return;
            }

            __instance.backgroundImage.color = new Color(0.2f, 0.2f, 0.2f);
        }
    }

    [HarmonyPatch(typeof(FreeChatInputField), nameof(FreeChatInputField.Awake))]
    public static class FreeChatInputField_Awake
    {
        public static void Postfix(FreeChatInputField __instance)
        {
            if (!LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.EnableDarkMode.Value)
            {
                return;
            }

            __instance.background.color = new Color(0.2f, 0.2f, 0.2f);

            var comp = __instance.background.GetComponent<ButtonRolloverHandler>();
            comp.OutColor = new Color(0.15f, 0.15f, 0.15f);
            comp.OverColor = new Color(0.25f, 0.25f, 0.25f);
            comp.UnselectedColor = new Color(0.15f, 0.15f, 0.15f);
            __instance.textArea.gameObject.GetComponent<TextMeshPro>().color = Color.white;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public static class MeetingHud_Start
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (!LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.EnableDarkMode.Value)
            {
                return;
            }
            __instance.meetingContents.transform.FindChild("PhoneUI").FindChild("baseColor").GetComponent<SpriteRenderer>().color = new Color(0.01f, 0.01f, 0.01f);
            __instance.Glass.color = new Color(0.7f, 0.7f, 0.7f, 0.3f);
            __instance.SkipVoteButton.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.4f, 0.4f);

            foreach (SpriteRenderer playerMaterialColors in __instance.PlayerColoredParts)
            {
                playerMaterialColors.color = new Color(0.25f, 0.25f, 0.25f);
                PlayerMaterial.SetColors(7, playerMaterialColors);
            }
        }
    }

    [HarmonyPatch(typeof(ChatNotification), nameof(ChatNotification.Awake))]
    public static class ChatNotification_Awake
    {
        public static void Postfix(ChatNotification __instance)
        {
            __instance.background.color = new Color(0.2f, 0.2f, 0.2f);
            __instance.chatText.color = Color.white;
        }
    }
}