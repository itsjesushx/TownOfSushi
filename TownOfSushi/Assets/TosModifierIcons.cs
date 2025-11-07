using UnityEngine;

namespace TownOfSushi.Assets;

public static class TOSModifierIcons
{
    private static readonly string iconPath = "TownOfSushi.Resources.ModifierIcons";

    public static LoadableAsset<Sprite> Aftermath { get; } = new LoadableResourceAsset($"{iconPath}.Aftermath.png");
    public static LoadableAsset<Sprite> Bait { get; } = new LoadableResourceAsset($"{iconPath}.Bait.png");
    public static LoadableAsset<Sprite> Armored { get; } = new LoadableResourceAsset($"{iconPath}.Armored.png");
    public static LoadableAsset<Sprite> ButtonBarry { get; } = new LoadableResourceAsset($"{iconPath}.ButtonBarry.png");
    public static LoadableAsset<Sprite> Celebrity { get; } = new LoadableResourceAsset($"{iconPath}.Celebrity.png");

    // Probably should be replaced in the future for a better icon.
    public static LoadableAsset<Sprite> Swapper { get; } = new LoadableResourceAsset($"{iconPath}.Swapper.png");


    public static LoadableAsset<Sprite> Diseased { get; } = new LoadableResourceAsset($"{iconPath}.Diseased.png");
    public static LoadableAsset<Sprite> Egotist { get; } = new LoadableResourceAsset($"{iconPath}.Egotist.png");
    public static LoadableAsset<Sprite> Frosty { get; } = new LoadableResourceAsset($"{iconPath}.Frosty.png");
    public static LoadableAsset<Sprite> Multitasker { get; } = new LoadableResourceAsset($"{iconPath}.Multitasker.png");
    public static LoadableAsset<Sprite> Noisemaker { get; } = new LoadableResourceAsset($"{iconPath}.Noisemaker.png");
    public static LoadableAsset<Sprite> Decay { get; } = new LoadableResourceAsset($"{iconPath}.Decay.png");
    public static LoadableAsset<Sprite> Scientist { get; } = new LoadableResourceAsset($"{iconPath}.Scientist.png");
    public static LoadableAsset<Sprite> Scout { get; } = new LoadableResourceAsset($"{iconPath}.Scout.png");
    public static LoadableAsset<Sprite> Taskmaster { get; } = new LoadableResourceAsset($"{iconPath}.Taskmaster.png");
    public static LoadableAsset<Sprite> Torch { get; } = new LoadableResourceAsset($"{iconPath}.Torch.png");

    public static LoadableAsset<Sprite> Disperser { get; } = new LoadableResourceAsset($"{iconPath}.Disperser.png");
    public static LoadableAsset<Sprite> DoubleShot { get; } = new LoadableResourceAsset($"{iconPath}.DoubleShot.png");
    public static LoadableAsset<Sprite> Saboteur { get; } = new LoadableResourceAsset($"{iconPath}.Saboteur.png");
    public static LoadableAsset<Sprite> Telepath { get; } = new LoadableResourceAsset($"{iconPath}.Telepath.png");
    public static LoadableAsset<Sprite> Underdog { get; } = new LoadableResourceAsset($"{iconPath}.Underdog.png");
    
    public static LoadableAsset<Sprite> Giant { get; } = new LoadableResourceAsset($"{iconPath}.Giant.png");
    public static LoadableAsset<Sprite> Lazy { get; } = new LoadableResourceAsset($"{iconPath}.Lazy.png");
    public static LoadableAsset<Sprite> Lover { get; } = new LoadableResourceAsset($"{iconPath}.Lover.png");
    public static LoadableAsset<Sprite> Mini { get; } = new LoadableResourceAsset($"{iconPath}.Mini.png");
    public static LoadableAsset<Sprite> Paranoiac { get; } = new LoadableResourceAsset($"{iconPath}.Paranoiac.png");
    public static LoadableAsset<Sprite> Satellite { get; } = new LoadableResourceAsset($"{iconPath}.Satellite.png");
    public static LoadableAsset<Sprite> Shy { get; } = new LoadableResourceAsset($"{iconPath}.Shy.png");
    public static LoadableAsset<Sprite> SixthSense { get; } = new LoadableResourceAsset($"{iconPath}.SixthSense.png");
    public static LoadableAsset<Sprite> Sleuth { get; } = new LoadableResourceAsset($"{iconPath}.Sleuth.png");
    public static LoadableAsset<Sprite> Tiebreaker { get; } = new LoadableResourceAsset($"{iconPath}.Tiebreaker.png");

    public static LoadableAsset<Sprite> FirstRoundShield { get; } =
        new LoadableResourceAsset($"{iconPath}.FirstRoundShield.png");
}