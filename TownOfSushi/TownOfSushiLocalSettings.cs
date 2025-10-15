using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using TownOfSushi.LocalSettings.Attributes;
using TownOfSushi.LocalSettings.SettingTypes;
using TownOfSushi.Patches;

namespace TownOfSushi;

public class TownOfSushiLocalSettings(ConfigFile config) : LocalSettingsTab(config)
{
  public override string TabName => "ToS";
  public static Dictionary<LocalizedLocalSliderSetting, string> LocalizedSliders { get; } = [];
  public static Dictionary<ToggleButtonBehaviour, string> LocalizedToggles { get; } = [];
  protected override bool ShouldCreateLabels => true;
  public override void Open()
  {
    base.Open();

    foreach (var entry in LocalizedToggles)
    {
      var toggleObject = entry.Key;
      LocalizedLocalToggleSetting.UpdateToggleText(toggleObject.Text, entry.Value, toggleObject.onState);
    }
    foreach (var entry in LocalizedSliders)
    {
      var sliderObject = entry.Key;
      sliderObject.SliderObject.Title.text = LocalizedLocalSliderSetting.GetLocalizedValueText(sliderObject, sliderObject.LocaleKey);
    }
  }

  public override void OnOptionChanged(ConfigEntryBase configEntry)
  {
    base.OnOptionChanged(configEntry);
    if (configEntry == ButtonUIFactorSlider)
    {
      var slider = LocalizedSliders.FirstOrDefault(x => x.Key.ConfigEntry == ButtonUIFactorSlider).Key;
      if (HudManager.InstanceExists)
      {
        HudManagerPatches.ResizeUI(1f / slider.OldValue);
        HudManagerPatches.ResizeUI(slider.GetValue());
      }
    }
  }

  public override LocalSettingTabAppearance TabAppearance => new()
  {
    TabIcon = TOSAssets.KillSprite,
  };

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> ShowVentsToggle { get; private set; } = config.Bind("Gameplay", "Show Vents In Map", true);
  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> SortGuessingByAlignmentToggle { get; private set; } = config.Bind("Gameplay", "Sort GuessingByAlignment", false);
  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> PreciseCooldownsToggle { get; private set; } = config.Bind("Gameplay", "Precise Cooldowns", false);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> ShowShieldHudToggle { get; private set; } = config.Bind("UI/Visuals", "Show Shield Hud", true);
  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> OffsetButtonsToggle { get; private set; } = config.Bind("UI/Visuals", "OffsetButtons", false);
  [LocalizedLocalSliderSetting(min: 0.5f, max: 1.5f, suffixType: MiraNumberSuffixes.Multiplier, formatString: "0.00", displayValue: true)]
  public ConfigEntry<float> ButtonUIFactorSlider { get; private set; } = config.Bind("UI/Visuals", "Button UI Factor", 0.75f);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> EnableDarkMode { get; private set; } = config.Bind("UI/Visuals", "Enable DarkMode", false);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> DisableNameplates { get; private set; } = config.Bind("UI/Visuals", "Disable Nameplates", false);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> DisableLevelIndicators { get; private set; } = config.Bind("UI/Visuals", "Disable Level Indicators", false);
  
  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> DisableLobbyMusic { get; private set; } = config.Bind("UI/Visuals", "Disable Lobby Music", false);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> UseCrewmateTeamColorToggle { get; private set; } = config.Bind("UI/Visuals", "Use Crewmate Team Color", false);

  [LocalizedLocalEnumSetting(names: ["Arrow Default", "Arrow Dark Glow", "Arrow Color Glow", "Arrow Legacy"])]
  public ConfigEntry<ArrowStyleType> ArrowStyleEnum { get; private set; } = config.Bind("UI/Visuals", "Arrow Style", ArrowStyleType.Default);


  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> ShowWelcomeMessageToggle { get; private set; } = config.Bind("Miscellaneous", "Show Welcome Message", true);

  [LocalizedLocalToggleSetting]
  public ConfigEntry<bool> ShowSummaryMessageToggle { get; private set; } = config.Bind("Miscellaneous", "Show Summary Message", true);

  [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> VanillaWikiEntriesToggle { get; private set; } = config.Bind("Miscellaneous", "Show Vanilla Wiki Entries", false);
}

public enum ArrowStyleType
{
    Default,
    DarkGlow,
    ColorGlow,
    Legacy
}