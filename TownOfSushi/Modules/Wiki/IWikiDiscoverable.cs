using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modules.Wiki;

public interface IWikiDiscoverable
{
    public string GetAdvancedDescription()
    {
        return MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities => [];
    
    public string SecondTabName => "Abilities";
}

public record struct CustomButtonWikiDescription(string name, string description, LoadableAsset<Sprite> icon);