using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SixthSenseModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Sixth Sense";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.SixthSense;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public string GetAdvancedDescription()
    {
        return
            "You will know when someone uses their ability on you.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Know when someone interacts with you.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SixthSenseOptions>.Instance.SixthSenseChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<SixthSenseOptions>.Instance.SixthSenseAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role is not AurialRole;
    }
}