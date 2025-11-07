using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Killer;

public sealed class RuthlessModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Ruthless";
    public override string IntroInfo => "your murder attemps cannot be stopped.";
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Saboteur;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;

    public string GetAdvancedDescription()
    {
        return
            "You can kill anyone even if they are protected"
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Your kills cannot be stopped!";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<RuthlessOptions>.Instance.RuthlessChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<RuthlessOptions>.Instance.RuthlessAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.Player.IsKillerRole();
    }
}