using BepInEx.Configuration;
using MiraAPI.LocalSettings;
using TownOfSushi.LocalSettings.Attributes;
using TownOfSushi.LocalSettings.SettingTypes;
using TownOfSushi.Patches;

namespace TownOfSushi;

public class TownOfSushiLocalSettings(ConfigFile config) : LocalSettingsTab(config)
{
    public override string TabName => "Town of Sushi";
    protected override bool ShouldCreateLabels => true;
  //   public override void Open()
  //   {
    //     base.Open();
        
     //    foreach (var entry in TuLocale.LocalizedToggles)
     //    {
      //       var toggleObject = entry.Key;
     //        LocalizedLocalToggleSetting.UpdateToggleText(toggleObject.Text, entry.Value, toggleObject.onState);
     //    }
     //    foreach (var entry in TouLocale.LocalizedSliders)
    //     {
       //      var sliderObject = entry.Key;
         //    sliderObject.SliderObject.Title.text = LocalizedLocalSliderSetting.GetLocalizedValueText(sliderObject, sliderObject.LocaleKey);
    //    }
  //  }

    public override void OnOptionChanged(ConfigEntryBase configEntry)
    {
        base.OnOptionChanged(configEntry);
        if (configEntry == ButtonUIFactorSlider && HudManager.InstanceExists)
        {
            HudManagerPatches.ResizeUI(1f / ButtonUIFactorSlider.Value);
            HudManagerPatches.ResizeUI(ButtonUIFactorSlider.Value);
        }
    }

    public override LocalSettingTabAppearance TabAppearance => new()
    {
        TabIcon = TOSAssets.KillSprite,
    };

    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> DeadSeeGhostsToggle { get; private set; } = config.Bind("Gameplay", "DeadSeeGhosts", true);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> ShowVentsToggle { get; private set; } = config.Bind("Gameplay", "ShowVents", true);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> SortGuessingByAlignmentToggle { get; private set; } = config.Bind("Gameplay", "SortGuessingByAlignment", false);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> PreciseCooldownsToggle { get; private set; } = config.Bind("Gameplay", "PreciseCooldowns", false);

    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> ShowShieldHudToggle { get; private set; } = config.Bind("UI/Visuals", "ShowShieldHud", true);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> OffsetButtonsToggle { get; private set; } = config.Bind("UI/Visuals", "OffsetButtons", false);
    [LocalizedLocalSliderSetting(min: 0.5f, max: 1.5f, suffixType: MiraNumberSuffixes.Multiplier, formatString: "0.00", displayValue:true)]
    public ConfigEntry<float> ButtonUIFactorSlider { get; private set; } = config.Bind("UI/Visuals", "ButtonUIFactor", 0.75f);

    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> ColorPlayerNameToggle { get; private set; } = config.Bind("UI/Visuals", "ColorPlayerName", false);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> UseCrewmateTeamColorToggle { get; private set; } = config.Bind("UI/Visuals", "UseCrewmateTeamColor", false);
    [LocalizedLocalEnumSetting(names:["ArrowDefault", "ArrowDarkGlow", "ArrowColorGlow", "ArrowLegacy"])]
    public ConfigEntry<ArrowStyleType> ArrowStyleEnum { get; private set; } = config.Bind("UI/Visuals", "ArrowStyle", ArrowStyleType.Default);

    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> ShowWelcomeMessageToggle { get; private set; } = config.Bind("Miscellaneous", "ShowWelcomeMessage", true);
    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> ShowSummaryMessageToggle { get; private set; } = config.Bind("Miscellaneous", "ShowSummaryMessage", true);

    [LocalizedLocalToggleSetting]
    public ConfigEntry<bool> VanillaWikiEntriesToggle { get; private set; } = config.Bind("Miscellaneous", "ShowVanillaWikiEntries", false);
}

public enum ArrowStyleType
{
    Default,
    DarkGlow,
    ColorGlow,
    Legacy
}