using UnityEngine;

namespace TownOfSushi.Assets;

public static class TOSCrewAssets
{
    private const string ShortPath = "TownOfSushi.Resources";
    private const string ButtonPath = $"{ShortPath}.CrewButtons";
    private const string BannerPath = $"{ShortPath}.RoleBanners";

    // THIS FILE SHOULD ONLY HOLD BUTTONS AND ROLE BANNERS, EVERYTHING ELSE BELONGS IN TOSAssets.cs
    public static LoadableAsset<Sprite> InspectSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.InspectButton.png");
    
    public static LoadableAsset<Sprite> Observe { get; } =
        new LoadableResourceAsset($"{ButtonPath}.ObserveButton.png");

    public static LoadableAsset<Sprite> GuardSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.GuardButton.png");

    public static LoadableAsset<Sprite> ExamineSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.ExamineButton.png");

    public static LoadableAsset<Sprite> WatchSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.WatchButton.png");
    
    public static LoadableAsset<Sprite> KnightSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.KnightButton.png");

    public static LoadableAsset<Sprite> ConfessSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.ConfessButton.png");

    public static LoadableAsset<Sprite> BlessSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.BlessButton.png");

    public static LoadableAsset<Sprite> DetectiveSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.DetectiveButton.png");

    public static LoadableAsset<Sprite> TrackSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.TrackButton.png");

    public static LoadableAsset<Sprite> SeerButton { get; } =
        new LoadableResourceAsset($"{ButtonPath}.SeerButton.png");

    public static LoadableAsset<Sprite> TrapSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.TrapButton.png");

    public static LoadableAsset<Sprite> CampButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CampButton.png");

    public static LoadableAsset<Sprite> StalkButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.StalkButton.png");

    public static LoadableAsset<Sprite> JailSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.JailButton.png");

    public static LoadableAsset<Sprite> AlertSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.AlertButton.png");

    public static LoadableAsset<Sprite> HunterKillSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.HunterKillButton.png");

    public static LoadableAsset<Sprite> VigilanteShootSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.VigilanteShootButton.png");

    public static LoadableAsset<Sprite> ReviveSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.ReviveButton.png");

    public static LoadableAsset<Sprite> CleanseSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CleanseButton.png");

    public static LoadableAsset<Sprite> BarrierSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.BarrierButton.png");

    public static LoadableAsset<Sprite> MedicSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.MedicButton.png");

    public static LoadableAsset<Sprite> FortifySprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.FortifyButton.png");

    public static LoadableAsset<Sprite> FixButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.FixButton.png");

    public static LoadableAsset<Sprite> EngiVentSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.EngiVentButton.png");

    public static LoadableAsset<Sprite> MediateSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.MediateButton.png");

    public static LoadableAsset<Sprite> CampaignButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CampaignButton.png");

    public static LoadableAsset<Sprite> FlushSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.FlushButton.png");

    public static LoadableAsset<Sprite> BarricadeSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.BarricadeButton.png");

    public static LoadableAsset<Sprite> Transport { get; } =
        new LoadableResourceAsset($"{ButtonPath}.TransportButton.png");

    public static LoadableAsset<Sprite> EngineerRoleBanner { get; } =
        new LoadableResourceAsset($"{BannerPath}.Engineer.png");
}