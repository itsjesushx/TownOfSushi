using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class GiantModifier : UniversalGameModifier, IWikiDiscoverable, IVisualAppearance
{
    public override string ModifierName => "Giant";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Giant;
    public override string GetDescription() => $"You are bigger than the average player, moving {Math.Round(1f / OptionGroupSingleton<GiantOptions>.Instance.GiantSpeed, 2)}x slower";
    public override ModifierFaction FactionType => ModifierFaction.UniversalVisibility;

    public override int GetAssignmentChance() =>
        (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.GiantChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.GiantAmount;

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = OptionGroupSingleton<GiantOptions>.Instance.GiantSpeed;
        appearance.Size = new Vector3(1f, 1f, 1f);
        return appearance;
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);
    }

    public override void OnDeactivate()
    {
        Player?.ResetAppearance(fullReset: true);
    }
    public string GetAdvancedDescription()
    {
        return
            $"You are bigger than regular players, and you also move {Math.Round(OptionGroupSingleton<GiantOptions>.Instance.GiantSpeed, 2)}x slower than regular players.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
