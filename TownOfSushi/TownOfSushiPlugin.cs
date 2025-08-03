using System.Globalization;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using MiraAPI;
using MiraAPI.PluginLoading;
using MiraAPI.Utilities.Assets;
using Reactor;
using Reactor.Localization;
using Reactor.Networking;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
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

    public static ConfigEntry<bool> DeadSeeGhosts { get; set; }
    public static ConfigEntry<bool> ShowShieldHud { get; set; }
    public static ConfigEntry<bool> ShowSummaryMessage { get; set; }
    public static ConfigEntry<bool> ShowWelcomeMessage { get; set; }
    public static ConfigEntry<bool> UseCrewmateTeamColor { get; set; }
    public static ConfigEntry<bool> ShowVents { get; set; }
    public static ConfigEntry<int> GameSummaryMode { get; set; }
    public static ConfigEntry<float> ButtonUIFactor { get; set; }
    public static ConfigEntry<bool> OffsetButtons { get; set; }
    public static ConfigEntry<bool> SortGuessingByAlignment { get; set; }
    public static ConfigEntry<bool> PreciseCooldowns { get; set; }
    public static ConfigEntry<int> ArrowStyle { get; set; }

    public static ConfigEntry<bool> EnableDarkMode { get; set; }
    public static ConfigEntry<bool> DisableNameplates { get; set; }
    public static ConfigEntry<bool> DisableLevelIndicators { get; set; }
    public static ConfigEntry<bool> DisableLobbyMusic { get; set; }

    /// <summary>
    ///     Determines if the current build is a dev build or not. This will change certain visuals as well as always grab news locally to be up to date.
    /// </summary>
    public static bool IsDevBuild => true;
    
    /// <inheritdoc />
    public string OptionsTitleText => "Town of Sushi";
    public const string DevString = " (Dev 11)";

    /// <inheritdoc />
    public ConfigFile GetConfigFile()
    {
        return Config;
    }

    public TownOfSushiPlugin()
    {
        TOSLocale.Initialize();
    }
    public static string RobotName { get; set; } = "Bot";
    public static bool Persistence { get; set; } = true;

    public static Debugger Debugger { get; set; }

    /// <summary>
    ///     The Load method for the plugin.
    /// </summary>
    public override void Load()
    {
        ReactorCredits.Register($"TownOfSushi v{Version}{DevString}", "", isPreRelease: true, ReactorCredits.AlwaysShow);
        LocalizationManager.Register(new TaskProvider());

        TOSAssets.Initialize();

        IL2CPPChainloader.Instance.Finished +=
            ModCompatibility
                .Initialize; // Initialise AFTER the mods are loaded to ensure maximum parity (no need for the soft dependency either then)
        IL2CPPChainloader.Instance.Finished +=
            ModNewsFetcher.CheckForNews; // Checks for mod announcements after everything is loaded to avoid Epic Games crashing

        var path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(TownOfSushiPlugin))!.Location) + "\\touhats.catalog";
        AddressablesLoader.RegisterCatalog(path);
        AddressablesLoader.RegisterHats("touhats");

        ClassInjector.RegisterTypeInIl2Cpp<Debugger>();
        ClassInjector.RegisterTypeInIl2Cpp<Component>();
        AddComponent<Component>();
        Debugger = AddComponent<Debugger>();

        DeadSeeGhosts = Config.Bind("LocalSettings", "DeadSeeGhosts", true, "If you see other ghosts when dead");
        ShowShieldHud = Config.Bind("LocalSettings", "ShowShieldHud", true,
            "If you see shield modifiers with a description, turn this off if it gets in your way.");
        ShowSummaryMessage = Config.Bind("LocalSettings", "ShowSummaryMessage", true,
            "If you see the game summary message when you join the lobby again.");
        ShowWelcomeMessage = Config.Bind("LocalSettings", "ShowWelcomeMessage", true,
            "If you see the welcome message when you first join a game.");
        UseCrewmateTeamColor = Config.Bind("LocalSettings", "UseCrewmateTeamColor", false,
            "Changes if all crewmate roles use the vanilla crewmate color instead.");
        ShowVents = Config.Bind("LocalSettings", "ShowVents", true, "If you see the vents on the minimap.");
        ButtonUIFactor = Config.Bind("LocalSettings", "ButtonUIFactor", 0.8f,
            "Scale factor for buttons in-game. Preferably, keep the value between 0.5f and 1.5f.");
        GameSummaryMode = Config.Bind("LocalSettings", "GameSummaryMode", 1,
            "How the Game Summary appears in the Win Screen. 0 is to the left, 1 is split, and 2 is hidden.");
        OffsetButtons = Config.Bind("LocalSettings", "OffsetButtons", false,
            "If venting is disabled (and you're not an impostor), should there be a blank spot where the vent button usually is?");
        SortGuessingByAlignment = Config.Bind("LocalSettings", "SortGuessingByAlignment", false,
            "Sorts the guessing menu by alignment alphabetically or purely alphabetical order.");
        PreciseCooldowns = Config.Bind("LocalSettings", "PreciseCooldowns", false,
            "Whether Button Cooldowns Show To 1 Decimal Place When It is Less Than 10 Seconds Remaining.");
        ArrowStyle = Config.Bind("LocalSettings", "ArrowStyle", 2,
            "How role arrows appear. 0 is the basic arrow, 1 is the arrow with a dark glow, 2 is the arrow with a light glow, and 3 is the legacy arrow / task arrow.");
        EnableDarkMode = Config.Bind("LocalSettings", "EnableDarkMode", false,
            "Whether the game chat and most of the UI stuff is black.");
        DisableNameplates = Config.Bind("LocalSettings", "DisableNameplates", false,
            "Whether to make everyone's nameplate the default one");
        DisableLevelIndicators = Config.Bind("LocalSettings", "DisableLevelIndicators", false,
            "Whether to remove the level indicators");
        DisableLobbyMusic = Config.Bind("LocalSettings", "DisableLobbyMusic", false,
            "Whether to remove the music in lobby");

        Harmony.PatchAll();
    }
}