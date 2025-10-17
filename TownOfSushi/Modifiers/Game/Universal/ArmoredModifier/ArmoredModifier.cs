using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ArmoredModifier : UniversalGameModifier, IWikiDiscoverable
{
    public static LoadableAsset<Sprite> Icon { get; } = TOSModifierIcons.Armored;
    public override string ModifierName => "Armored";
    public override LoadableAsset<Sprite>? ModifierIcon => Icon;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;
    public override Color FreeplayFileColor => TownOfSushiColors.Armored; // Hex Color = #948851

    public bool isActive = true;

    public string GetAdvancedDescription()
    {
        return $"If you are attacked and have no shields, you will prevent the kill and put your killer's ability on {OptionGroupSingleton<ArmoredOptions>.Instance.ResetCooldown}s cooldown.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You can survive the first attack you recieve during rounds.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ArmoredOptions>.Instance.ArmoredChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ArmoredOptions>.Instance.ArmoredAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (role is PestilenceRole) // Pestilence should not get this modifier.
        {
            return false;
        }

        return base.IsModifierValidOn(role);
    }
}