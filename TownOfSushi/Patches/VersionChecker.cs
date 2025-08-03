using System.Text.Json;
using HarmonyLib;
using Twitch;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
public static class VersionCheckPatch
{
    public static void Prefix()
    {
        var data = GetVersioning()
            ?.FirstOrDefault(x => x.ModVersion.Equals(TownOfSushiPlugin.Version, StringComparison.Ordinal));
        if (data != null)
        {
            var RequiredVersions = data.InternalVersions;
            var AUversion = Constants.GetBroadcastVersion();
            if (!RequiredVersions.ContainsKey(AUversion))
            {
                var action = AUversion > RequiredVersions.Keys.Max() ? "downgrade" : "update";
                var info =
                    $"ALERT\nTown of Sushi {TownOfSushiPlugin.Version} requires {RequiredVersions.Values.Last()}\nyou have {Application.version}\nPlease {action} your among us version"
                    + "\nvisit Github or Discord for any help";
                var man = TwitchManager.Instance;
                ModUpdater.InfoPopup = Object.Instantiate(man.TwitchPopup);
                ModUpdater.InfoPopup.TextAreaTMP.fontSize *= 0.68f;
                ModUpdater.InfoPopup.TextAreaTMP.enableAutoSizing = true;
                ModUpdater.InfoPopup.Show(info);
                ModUpdater.InfoPopup.StartCoroutine(Effects.Lerp(0.01f,
                    new Action<float>(p => { ModUpdater.setPopupText(info); })));
                ModUpdater.InvalidAUVersion = true;
            }
        }
    }

    private static List<ModUpdater.UpdateData>? GetVersioning()
    {
#pragma warning disable S1075 // URIs should not be hardcoded
        var text = ModUpdater.Httpclient
            .GetAsync("https://github.com/AU-Avengers/TOU-Mira/raw/refs/heads/dev/Versioning.json")
            .GetAwaiter().GetResult().Content.ReadAsStringAsync().Result;
#pragma warning restore S1075 // URIs should not be hardcoded
        var data = JsonSerializer.Deserialize<List<ModUpdater.UpdateData>>(text,
            options: new() { ReadCommentHandling = JsonCommentHandling.Skip });
        return data;
    }
}

public class ModUpdater
{
    public static bool InvalidAUVersion;
    public static GenericPopup InfoPopup;

    public static HttpClient Httpclient = new()
    {
        DefaultRequestHeaders =
        {
            { "User-Agent", "TownOfSushi Updater" }
        }
    };

    public static void setPopupText(string message)
    {
        if (InfoPopup == null)
        {
            return;
        }

        if (InfoPopup.TextAreaTMP != null)
        {
            InfoPopup.TextAreaTMP.text = message;
        }
    }

    public class UpdateData
    {
        public Dictionary<int, string> InternalVersions { get; set; }

        public string ModVersion { get; set; }
    }
}

[HarmonyPatch(typeof(GenericPopup), nameof(GenericPopup.Close))]
public static class TextBoxPatch
{
    [HarmonyPostfix]
    public static void Postfix(GenericPopup __instance)
    {
        if (__instance != ModUpdater.InfoPopup)
        {
            return;
        }

        if (ModUpdater.InvalidAUVersion)
        {
            Application.Quit();
        }
    }
}