using Reactor.Utilities;
using UnityEngine;

namespace TownOfSushi.Assets;

public static class AnimationMaterials
{
    public static AssetBundle TrapperShaderBundle { get; } =
        AssetBundleManager.Load(typeof(AnimationMaterials).Assembly, "trappershader");

    public static AssetBundle SoundVisionBundle { get; } =
        AssetBundleManager.Load(typeof(AnimationMaterials).Assembly, "soundvision");

    // bomb visualizer thing
    public static LoadableAsset<Material> BombMaterial { get; private set; }
    public static LoadableAsset<Material> IgniteMaterial { get; private set; }
    public static LoadableAsset<Material> TrapMaterial { get; private set; }

    public static void Initialize()
    {
        BombMaterial = new LoadableBundleAsset<Material>("bomb", TrapperShaderBundle);
        IgniteMaterial = new LoadableBundleAsset<Material>("pyromaniactrap", TrapperShaderBundle);
        TrapMaterial = new LoadableBundleAsset<Material>("trap", TrapperShaderBundle);
    }
}