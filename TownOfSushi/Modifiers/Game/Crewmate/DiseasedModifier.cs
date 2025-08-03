using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DiseasedModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Diseased";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Diseased;
    public override string GetDescription() => "Increase your killer's kill cooldown.";
    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DiseasedChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.DiseasedAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
    public string GetAdvancedDescription()
    {
        return
            $"After you die, your killer's kill cooldown is multiplied by a factor of {OptionGroupSingleton<DiseasedOptions>.Instance.CooldownMultiplier}x.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
