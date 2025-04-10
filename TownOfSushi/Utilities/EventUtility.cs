﻿using System;


namespace TownOfSushi.Utilities;

[HarmonyPatch]
public static class EventUtility 
{

    public static void Load() 
    {
        if (!isEnabled) return;
    }

    public static void ClearAndReload() 
    {
    }

    public static void Update() 
    {
        if (!isEnabled || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started || TownOfSushi.rnd == null || IntroCutscene.Instance) return;
    }

    public static DateTime enabled = DateTime.FromBinary(638475264000000000);
    public static bool isEventDate => DateTime.Today.Date == enabled;

    public static bool canBeEnabled => DateTime.Today.Date >= enabled && DateTime.Today.Date <= enabled.AddDays(7); // One Week after the EVENT
    public static bool isEnabled => isEventDate || canBeEnabled && CustomOptionHolder.enableEventMode != null && CustomOptionHolder.enableEventMode.GetBool();

    public static void MeetingEndsUpdate() 
    {
        if (!isEnabled) return;
        // TODO - Implement Horse hats
        // PlayerControl.LocalPlayer.RpcSetHat(CustomHatLoader.horseHatProductIds[rnd.Next(CustomHatLoader.horseHatProductIds.Count)]);
    }


    public static void MeetingStartsUpdate() 
    {
        if (!isEnabled) return;
    }

    public static void GameStartsUpdate() 
    {
        if (!isEnabled) return;
    }

    public static void GameEndsUpdate() 
    {
        if (!isEnabled) return;
    }


    [HarmonyPatch(typeof(ChatController), nameof(ChatController.AddChat))]
    public static class AddChatPatch 
    {
        public static void Prefix(ChatController __instance, PlayerControl sourcePlayer, ref string chatText, bool censor) 
        {
            if (!isEnabled) return;
            var charArray = chatText.ToCharArray();
            Array.Reverse(charArray);
            chatText = new string(charArray);
        }
    }
}