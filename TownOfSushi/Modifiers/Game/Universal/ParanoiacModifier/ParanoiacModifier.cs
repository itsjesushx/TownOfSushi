using Reactor.Utilities.Extensions;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ParanoiacModifier : UniversalGameModifier, IWikiDiscoverable
{
    private ArrowBehaviour _arrow;
    public override string ModifierName => "Paranoiac";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Paranoiac;

    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public string GetAdvancedDescription()
    {
        return
            "Get an arrow to the closest player.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You have an arrow pointing\n to the closest player.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ParanoiacOptions>.Instance.ParanoiacChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ParanoiacOptions>.Instance.ParanoiacAmount;
    }

    public override void OnActivate()
    {
        _arrow = MiscUtils.CreateArrow(Player.gameObject.transform, new Color(1f, 0f, 0.5f, 1f));
    }

    public override void OnDeactivate()
    {
        if (_arrow)
        {
            _arrow.gameObject.Destroy();
        }
    }

    public override void FixedUpdate()
    {
        if (!Player.AmOwner ||
            !Player.Data ||
            Player.Data.IsDead)
        {
            _arrow.gameObject.SetActive(false);
            return;
        }

        var target = Helpers.GetClosestPlayers(Player, float.MaxValue)
            .FirstOrDefault(playerInfo => !playerInfo.Data.Disconnected &&
                                          playerInfo.PlayerId != Player.PlayerId &&
                                          ((playerInfo.TryGetModifier<DisabledModifier>(out var mod) &&
                                            mod.IsConsideredAlive) || !playerInfo.HasModifier<DisabledModifier>()) &&
                                          !playerInfo.Data.IsDead);
        if (!target)
        {
            return;
        }

        _arrow.gameObject.SetActive(true);
        _arrow.target = target!.transform.localPosition;
    }
}