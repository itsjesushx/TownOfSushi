using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace TownOfSushi.Assets;

public static class TOSImpAssets
{
    private const string ShortPath = "TownOfSushi.Resources";
    private const string BannerPath = $"{ShortPath}.RoleBanners";
    private const string ButtonPath = $"{ShortPath}.ImpButtons";

    // THIS FILE SHOULD ONLY HOLD BUTTONS AND ROLE BANNERS, EVERYTHING ELSE BELONGS IN TOSAssets.cs
    public static LoadableAsset<Sprite> MarkSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.MarkButton.png");

    public static LoadableAsset<Sprite> RecallSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.RecallButton.png");
    
    public static LoadableAsset<Sprite> AmbushSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.AmbushButton.png");

    public static LoadableAsset<Sprite> PursueSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.PursueButton.png");
    
    public static LoadableAsset<Sprite> WarlockCurseButton { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CurseButton.png");
    
    public static LoadableAsset<Sprite> WarlockCurseKillButton { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CurseKillButton.png", 80f);

    public static LoadableAsset<Sprite> FlashSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.FlashButton.png");
    
    public static LoadableAsset<Sprite> CurseSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.SpellButton.png");

    public static LoadableAsset<Sprite> SampleSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.SampleButton.png");

    public static LoadableAsset<Sprite> MorphSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.MorphButton.png");

    public static LoadableAsset<Sprite> SwoopSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.SwoopButton.png");

    public static LoadableAsset<Sprite> UnswoopSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.UnswoopButton.png");

    public static LoadableAsset<Sprite> NoAbilitySprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.NoAbilityButton.png");

    public static LoadableAsset<Sprite> CamouflageSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CamouflageButton.png");

    public static LoadableAsset<Sprite> SprintSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CamoSprintButton.png");

    public static LoadableAsset<Sprite> FreezeSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CamoSprintFreezeButton.png");

    public static LoadableAsset<Sprite> PlaceSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.PlaceButton.png");

    public static LoadableAsset<Sprite> DetonatingSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.DetonatingButton.png");

    public static LoadableAsset<Sprite> PlantSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.PlantButton.png");

    public static LoadableAsset<Sprite> TraitorSelect { get; } =
        new LoadableResourceAsset($"{ButtonPath}.TraitorSelect.png");

    public static LoadableAsset<Sprite> BlackmailSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.BlackmailButton.png");

    public static LoadableAsset<Sprite> HypnotiseButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.HypnotiseButton.png");
    
    public static LoadableAsset<Sprite> PoisonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.PoisonButton.png");

    public static LoadableAsset<Sprite> CleanButtonSprite { get; } =
        new LoadableResourceAsset($"{ButtonPath}.CleanButton.png");

    public static LoadableAsset<Sprite> MineSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.MineButton.png");
    public static LoadableAsset<Sprite> DragSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.DragButton.png");
    public static LoadableAsset<Sprite> DropSprite { get; } = new LoadableResourceAsset($"{ButtonPath}.DropButton.png");

    public static LoadableAsset<Sprite> MinerRoleBanner { get; } = new LoadableResourceAsset($"{BannerPath}.Miner.png");
}