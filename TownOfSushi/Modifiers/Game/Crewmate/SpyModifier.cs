using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Buttons.Modifiers;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Options.Modifiers.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;
using static ShipStatus;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class SpyModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Spy";
    public override LoadableAsset<Sprite>? ModifierIcon => TosRoleIcons.Spy;
    public override string GetDescription() => "Gain extra information on the Admin Table.";
    public override ModifierFaction FactionType => ModifierFaction.CrewmateUtility;
    public override void OnActivate()
    {
        base.OnActivate();

        if (!Player.AmOwner) return;
        CustomButtonSingleton<SpyAdminTableModifierButton>.Instance.AvailableCharge = OptionGroupSingleton<SpyOptions>.Instance.StartingCharge.Value;
    }
    public static void OnRoundStart()
    {
        CustomButtonSingleton<SpyAdminTableModifierButton>.Instance.AvailableCharge += OptionGroupSingleton<SpyOptions>.Instance.RoundCharge.Value;
    }
    public static void OnTaskComplete()
    {
        CustomButtonSingleton<SpyAdminTableModifierButton>.Instance.AvailableCharge += OptionGroupSingleton<SpyOptions>.Instance.TaskCharge.Value;
    }

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.SpyChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.SpyAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() && Instance.Type != MapType.Fungle;
    }
    public string GetAdvancedDescription()
    {
        return
            "The Spy gains extra information on the admin table. They now not only see how many people are in a room, but will also see who is in every room."
            + MiscUtils.AppendOptionsText(GetType());
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
