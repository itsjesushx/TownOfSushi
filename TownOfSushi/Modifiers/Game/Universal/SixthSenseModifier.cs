using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class SixthSenseModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Sixth Sense";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.SixthSense;
    public override string GetDescription() => "Know when someone interacts with you.";
    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SixthSenseChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.SixthSenseAmount;
    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role is not AurialRole;
    }
    public string GetAdvancedDescription()
    {
        return
            "You will know when someone uses their ability on you.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
