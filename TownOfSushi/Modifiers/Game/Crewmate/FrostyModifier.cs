using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class FrostyModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Frosty";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Frosty;
    public override string GetDescription() => "Slow your killer for a short duration.";
    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.FrostyChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.FrostyAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
    public string GetAdvancedDescription()
    {
        return
            "After you die, your killer will be slowed down!"
               + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
