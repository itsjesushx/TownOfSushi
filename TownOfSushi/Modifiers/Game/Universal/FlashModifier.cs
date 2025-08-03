﻿using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class FlashModifier : UniversalGameModifier, IWikiDiscoverable, IVisualAppearance
{
    public override string ModifierName => "Flash";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Flash;

    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = OptionGroupSingleton<FlashOptions>.Instance.FlashSpeed;
        return appearance;
    }

    public string GetAdvancedDescription()
    {
        return
            $"You move {Math.Round(OptionGroupSingleton<FlashOptions>.Instance.FlashSpeed, 2)}x faster than regular players.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return $"You move {Math.Round(OptionGroupSingleton<FlashOptions>.Instance.FlashSpeed, 2)}x faster.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.FlashChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.FlashAmount;
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
    }

    public override void OnDeactivate()
    {
        Player?.ResetAppearance(fullReset: true);
    }
}