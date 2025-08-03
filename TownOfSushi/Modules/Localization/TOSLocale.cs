using BepInEx.Logging;
using UnityEngine;

namespace TownOfSushi.Modules.Localization;

public static class TOSLocale
{
    public static string LocaleDirectory => Path.Combine(Application.persistentDataPath, "TownOfSushi", "Locales");

    public static Dictionary<SupportedLangs, Dictionary<TOSNames, string>> TOSLocalization { get; } = [];

    private static ManualLogSource Logger { get; } = BepInEx.Logging.Logger.CreateLogSource("TOSLocale");

    public static string Get(TOSNames name, string? defaultValue = null)
    {
        var currentLanguage = 
            TranslationController.InstanceExists ? 
                TranslationController.Instance.currentLanguage.languageID : 
                SupportedLangs.English;

        if (!TOSLocalization.TryGetValue(currentLanguage, out var translations) ||
            !translations.TryGetValue(name, out var translation))
        {
            return defaultValue ?? "STRMISS_" + name;
        }

        return translation;
    }

    public static void Initialize()
    {
        SearchDirectory(BepInEx.Paths.PluginPath);
        SearchDirectory(BepInEx.Paths.BepInExRootPath);
        SearchDirectory(BepInEx.Paths.GameRootPath);
        SearchDirectory(LocaleDirectory);
    }

    public static void SearchDirectory(string directory)
    {
        if (!Directory.Exists(directory))
        {
            Logger.LogWarning($"Directory does not exist: {directory}");
            return;
        }

        var translations = Directory.GetFiles(directory, "*.txt");
        foreach (var file in translations)
        {
            var localeName = Path.GetFileNameWithoutExtension(file);
            if (!Enum.TryParse<SupportedLangs>(localeName, out var language))
            {
                Logger.LogWarning($"Invalid locale name: {localeName}");
                continue;
            }

            TOSLocalization.TryAdd(language, []);
            ParseFile(file, language);
        }
    }

    public static void ParseFile(string file, SupportedLangs language)
    {
        foreach (var translation in File.ReadAllLines(file))
        {
            var parts = translation.Split('=');
            if (parts.Length >= 2)
            {
                var key = parts[0];
                var value = string.Join("=", parts.Skip(1));

                if (!Enum.TryParse<TOSNames>(key, out var touName))
                {
                    Logger.LogWarning("Invalid key value in translation: " + translation);
                }

                TOSLocalization[language].TryAdd(touName, value);
            }
            else
            {
                Logger.LogWarning("Invalid translation format: " + translation);
            }
        }
    }
}