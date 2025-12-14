using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public sealed class SaboteurModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Saboteur";
    public override string IntroInfo => "You have reduced sabotage cooldowns";
    public override Color FreeplayFileColor => new Color32(255, 25, 25, 255);

    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Saboteur;
    public override ModifierFaction FactionType => ModifierFaction.ImpostorPassive;

    public float Timer { get; set; }

    public string GetAdvancedDescription()
    {
        return
            "You have a reduced cooldown when sabotaging."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You have reduced sabotage cooldowns";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<SaboteurOptions>.Instance.SaboteurChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<SaboteurOptions>.Instance.SaboteurAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsImpostor();
    }
}