using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class DiseasedModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Diseased";
    public override string IntroInfo => "You will also extend your killer's cooldown upon death.";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Diseased;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);
    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;
    public string GetAdvancedDescription()
    {
        return
            $"After you die, your killer's kill cooldown is multiplied by a factor of {OptionGroupSingleton<DiseasedOptions>.Instance.CooldownMultiplier}x.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Increase your killer's kill cooldown.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<DiseasedOptions>.Instance.DiseasedChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<DiseasedOptions>.Instance.DiseasedAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
}