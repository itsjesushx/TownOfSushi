using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class DoubleShotModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Double Shot";
    public override string GetDescription() => "You have an extra chance when assassinating";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.DoubleShot;
    public override ModifierFaction FactionType => ModifierFaction.KillerUtility;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.DoubleShotChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.DoubleShotAmount;

    public bool Used { get; set; }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (
            role.Player.IsImpostor() 
            && role.Player.GetModifierComponent().HasModifier<ImpostorAssassinModifier>(true)
            && base.IsModifierValidOn(role)
            )
        {
            return true;
        }
        return false;
    }
    public string GetAdvancedDescription()
    {
        return
            "You get a second chance when you fail to shoot.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
