using Il2CppInterop.Runtime.Attributes;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Universal;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ButtonBarryModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Button Barry";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.ButtonBarry;
    public override string GetDescription() => "You can call a meeting\n from anywhere on the map.";
    public int Priority { get; set; } = 5;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;

    public override int GetAmountPerGame() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ButtonBarryAmount != 0 ? 1 : 0;
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ButtonBarryChance;
    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (role is SwapperRole && !OptionGroupSingleton<SwapperOptions>.Instance.CanButton)
        {
            return false;
        }
        else if (role is JesterRole && !OptionGroupSingleton<JesterOptions>.Instance.CanButton)
        {
            return false;
        }
        else if (role is ExecutionerRole && !OptionGroupSingleton<ExecutionerOptions>.Instance.CanButton)
        {
            return false;
        }

        return base.IsModifierValidOn(role);
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<BarryButton>.Instance.Usable = true;
    }

    public string GetAdvancedDescription()
    {
        return "You can button from anywhere on the map."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } = [
        new("Button",
            $"You can trigger an emergency meeting from across the map, which you may do {OptionGroupSingleton<ButtonBarryOptions>.Instance.MaxNumButtons} time(s) per game.",
            TosAssets.BarryButtonSprite),
    ];
    
}
