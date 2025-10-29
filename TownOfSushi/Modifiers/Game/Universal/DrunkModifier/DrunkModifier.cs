using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class DrunkModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Drunk";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Drunk;
    public override string GetDescription() => $"You have inverrrrrted controls for {Math.Round(OptionGroupSingleton<DrunkOptions>.Instance.DrunkDuration)} rounds.";
    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;

    public int DrunkDuration = (int)OptionGroupSingleton<DrunkOptions>.Instance.DrunkDuration;
    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<DrunkOptions>.Instance.DrunkChance;
    }
    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<DrunkOptions>.Instance.DrunkAmount;
    }
    public string GetAdvancedDescription()
    {
        return
            $"You have inverrrrrted controls for {Math.Round(OptionGroupSingleton<DrunkOptions>.Instance.DrunkDuration)} rounds.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}