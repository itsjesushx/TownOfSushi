using Il2CppInterop.Runtime.Attributes;
using MiraAPI.Hud;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ButtonBarryModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Button Barry";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.ButtonBarry;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public int Priority { get; set; } = 5;
    public override ModifierFaction FactionType => ModifierFaction.UniversalUtility;

    public string GetAdvancedDescription()
    {
        return "You can button from anywhere on the map."
               + MiscUtils.AppendOptionsText(GetType());
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities { get; } =
    [
        new("Button",
            $"You can trigger an emergency meeting from across the map, which you may do {OptionGroupSingleton<ButtonBarryOptions>.Instance.MaxNumButtons} time(s) per game.",
            TOSAssets.BarryButtonSprite)
    ];

    public override string GetDescription()
    {
        return "You can call a meeting\n from anywhere on the map.";
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<ButtonBarryOptions>.Instance.ButtonBarryAmount != 0 ? 1 : 0;
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<ButtonBarryOptions>.Instance.ButtonBarryChance;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        if (role is JesterRole && !OptionGroupSingleton<JesterOptions>.Instance.CanButton)
        {
            return false;
        }

        if (role is ExecutionerRole && !OptionGroupSingleton<ExecutionerOptions>.Instance.CanButton)
        {
            return false;
        }

        return base.IsModifierValidOn(role);
    }

    public static void OnRoundStart()
    {
        CustomButtonSingleton<BarryButton>.Instance.Usable = true;
    }
}