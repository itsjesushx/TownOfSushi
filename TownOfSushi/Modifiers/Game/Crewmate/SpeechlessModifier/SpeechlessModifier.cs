using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class SpeechlessModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Speechless";
    public override string IntroInfo => "You cannot report bodies.";
    public override LoadableAsset<Sprite>? ModifierIcon => MiraAssets.Empty;
    public override Color FreeplayFileColor => Color.gray;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;

    public string GetAdvancedDescription()
    {
        return
            "You physically cannot report bodies."
            + Utils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You cannot report bodies.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SpeechlessOptions>.Instance.SpeechlessChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<SpeechlessOptions>.Instance.SpeechlessAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role is not (InformantRole or InspectorRole or MysticRole);
    }
}