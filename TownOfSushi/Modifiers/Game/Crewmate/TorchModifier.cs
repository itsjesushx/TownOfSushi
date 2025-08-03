using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;
using static ShipStatus;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public sealed class TorchModifier : TosGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Torch";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Torch;
    public override string GetDescription() => "Your vision wont get reduced\nwhen the lights are sabotaged.";
    public override ModifierFaction FactionType => ModifierFaction.CrewmateVisibility;

    public override int GetAssignmentChance() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.TorchChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<CrewmateModifierOptions>.Instance.TorchAmount;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && role.IsCrewmate() && Instance.Type != MapType.Fungle;
    }
    public string GetAdvancedDescription()
    {
        return
            "The lights being off do not affect your vision.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
