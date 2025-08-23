﻿using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class TiebreakerModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Tiebreaker";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Tiebreaker;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public string GetAdvancedDescription()
    {
        return
            "Your vote allows you to break ties.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Your vote breaks ties";
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<TiebreakerOptions>.Instance.TiebreakerAmount != 0 ? 1 : 0;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<TiebreakerOptions>.Instance.TiebreakerChance;
    }
}