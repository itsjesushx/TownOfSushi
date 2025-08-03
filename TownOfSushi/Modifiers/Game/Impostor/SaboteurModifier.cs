using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class SaboteurModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Saboteur";
    public override string GetDescription() => "You have reduced sabotage cooldowns";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Saboteur;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPassive;

    public float Timer { get; set; }

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.SaboteurChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<ImpostorModifierOptions>.Instance.SaboteurAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();
    }
    public string GetAdvancedDescription()
    {
        return
            "You have a reduced cooldown when sabotaging."
               + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
