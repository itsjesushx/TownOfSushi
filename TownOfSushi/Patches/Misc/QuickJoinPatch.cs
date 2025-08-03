using AmongUs.Data;
using HarmonyLib;
using InnerNet;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace TownOfSushi.Patches.Misc;

[HarmonyPatch]

public static class LobbyJoin
{
    static int GameId;

    static GameObject LobbyText;

    static TextMeshPro Text;
    static bool JoiningAttempted;

    [HarmonyPatch(typeof(InnerNetClient), nameof(InnerNetClient.JoinGame))]
    [HarmonyPostfix]

    public static void Postfix(InnerNetClient __instance)
    {
        GameId = __instance.GameId;
    }

    [HarmonyPatch(typeof(EnterCodeManager), nameof(EnterCodeManager.OnEnable))]
    [HarmonyPostfix]

    public static void OnEnable(EnterCodeManager __instance)
    {
        if (LobbyText)
        {
            LobbyText.SetActive(GameId != 0);
            return;
        }

        LobbyText = UnityEngine.Object.Instantiate(__instance.transform.FindChild("Header").gameObject, __instance.transform);
        LobbyText.name = "LobbyText";
        Text = LobbyText.transform.GetChild(1).GetComponent<TextMeshPro>();
        Text.fontSizeMin = 3.35f;
        Text.fontSizeMax = 3.35f;
        Text.fontSize = 3.35f;
        Text.text = string.Empty;
        Text.alignment = TextAlignmentOptions.Center;
        LobbyText.transform.localPosition = new(1f, 0f, 0f);
        LobbyText.transform.GetChild(0).gameObject.Destroy();
        LobbyText.SetActive(GameId != 0);
    }

    [HarmonyPatch(typeof(EnterCodeManager), nameof(EnterCodeManager.OnDisable))]
    [HarmonyPostfix]

    public static void OnDisable()
    {
        if (LobbyText)
        {
            LobbyText.SetActive(false);
            JoiningAttempted = false;
        }
    }

    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.LateUpdate))]
    [HarmonyPostfix]

    public static void Update()
    {
        if (GameId == 0 || !LobbyText || !LobbyText.active) return;

        if (Input.GetKeyDown(KeyCode.Tab) && !JoiningAttempted)
        {
            AmongUsClient.Instance.StartCoroutine(AmongUsClient.Instance.CoFindGameInfoFromCodeAndJoin(GameId));
        }
        if (LobbyText && Text)
        {
            var code = GameCode.IntToGameName(GameId);
            if (DataManager.Settings.Gameplay.StreamerMode) code = "******";
            Text.text = $"<size=110%>Prev Lobby:</size>"
            + $"\n<size=4.6f>({code})</size>"
            + $"\nPress Tab to\n<size=2.6f>attempt joining</size>";
        }
    }
}
