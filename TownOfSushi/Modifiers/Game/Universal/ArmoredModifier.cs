using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfUs.Modules.Wiki;
using UnityEngine;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ArmoredModifier : UniversalGameModifier, IWikiDiscoverable
{
    public static LoadableAsset<Sprite> Icon { get; } = new LoadableResourceAsset($"TownOfSushi.Resources.ModifierIcons.Armored.png"); // I would change this later.
    public static Color Color => new(0.58f, 0.53f, 0.32f, 1f);
    public override string ModifierName => "Armored";
    public override LoadableAsset<Sprite>? ModifierIcon => Icon;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;
    public override Color FreeplayFileColor => Color; // Hex Color = #948851

    public bool isActive = true;

    public string GetAdvancedDescription()
    {
        return $"If you are attacked and have no shields, you will prevent the kill and put your killer's ability on {OptionGroupSingleton<ArmoredOptions>.Instance.ResetCooldown}s cooldown.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return $"You can survive the first attack you recieve during rounds."+ MiscUtils.AppendOptionsText(GetType());
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ArmoredChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ArmoredAmount;
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