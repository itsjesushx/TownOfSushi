using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class FrostyModifier : TOSGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Frosty";
    public override string IntroInfo => "You will also slow down your killer upon death.";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Frosty;
    public override Color FreeplayFileColor => new Color32(140, 255, 255, 255);

    public override ModifierFaction FactionType => ModifierFaction.CrewmatePostmortem;

    public string GetAdvancedDescription()
    {
        return
            "After you die, your killer will be slowed down!"
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "Slow your killer for a short duration.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<FrostyOptions>.Instance.FrostyChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<FrostyOptions>.Instance.FrostyAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate();
    }
}