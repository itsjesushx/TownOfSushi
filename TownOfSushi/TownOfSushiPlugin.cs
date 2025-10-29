using System.Globalization;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MiraAPI;
using MiraAPI.PluginLoading;
using Reactor;
using Reactor.Localization;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using TownOfSushi.Modules.Debugger.Components;
using TownOfSushi.Patches.Misc;
using static TownOfSushi.Modules.Debugger.Embedded.ReactorCoroutines.Coroutines;
using ModCompatibility = TownOfSushi.Modules.ModCompatibility;

namespace TownOfSushi;

/// <summary>
/// Plugin class for Town of Sushi.
/// </summary>
[BepInAutoPlugin("itsjesushx.tos", "Town of Sushi")]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
[BepInDependency(MiraApiPlugin.Id)]
[ReactorModFlags(ModFlags.RequireOnAllClients)]
public partial class TownOfSushiPlugin : BasePlugin, IMiraPlugin
{
    /// <summary>
    ///     Gets the specified Culture for string manipulations.
    /// </summary>
    public static CultureInfo Culture { get; } = new("en-US");

    /// <summary>
    ///     Gets the Harmony instance for patching.
    /// </summary>
    public Harmony Harmony { get; } = new(Id);

    public static ConfigEntry<int> GameSummaryMode { get; set; }

    /// <summary>
    ///     Determines if the current build is a dev build or not. This will change certain visuals as well as always grab news locally to be up to date.
    /// </summary>
    public static bool IsDevBuild => false;
    
    /// <inheritdoc />
    public string OptionsTitleText => "Town of Sushi";
    public const string DevString = "";

    /// <inheritdoc />
    public ConfigFile GetConfigFile()
    {
        return Config;
    }
    public static string RobotName { get; set; } = "Bot";
    public static bool Persistence { get; set; } = true;

    public static Debugger Debugger { get; set; }

    /// <summary>
    ///     The Load method for the plugin.
    /// </summary>
    public override void Load()
    {
        //ReactorCredits.Register($"TownOfSushi v", Version + DevString, isPreRelease: false, ReactorCredits.AlwaysShow);
        LocalizationManager.Register(new TaskProvider());

        TOSAssets.Initialize();

        IL2CPPChainloader.Instance.Finished +=
            ModCompatibility
                .Initialize; // Initialise AFTER the mods are loaded to ensure maximum parity (no need for the soft dependency either then)
        IL2CPPChainloader.Instance.Finished +=
            ModNewsFetcher.CheckForNews; // Checks for mod announcements after everything is loaded to avoid Epic Games crashing

        var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfSushiPlugin))!.Location) + "\\toshats.catalog";
        AddressablesLoader.RegisterCatalog(path);
        AddressablesLoader.RegisterHats("toshats");

        ClassInjector.RegisterTypeInIl2Cpp<Debugger>();
        ClassInjector.RegisterTypeInIl2Cpp<Component>();
        AddComponent<Component>();
        Debugger = AddComponent<Debugger>();

        GameSummaryMode = Config.Bind("LocalSettings", "GameSummaryMode", 1,
            "How the Game Summary appears in the Win Screen. 0 is to the left, 1 is split, and 2 is hidden.");

        Harmony.PatchAll();
    }
}