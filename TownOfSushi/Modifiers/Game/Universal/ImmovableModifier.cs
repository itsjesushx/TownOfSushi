using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modules;
using TownOfSushi.Modules.Wiki;
using TownOfSushi.Options.Modifiers;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class ImmovableModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Immovable";
    public override LoadableAsset<Sprite>? ModifierIcon => TosModifierIcons.Immovable;
    public override string GetDescription() => "You are unable to be moved via abilities and meetings.";
    public override int GetAssignmentChance() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ImmovableChance;
    public override int GetAmountPerGame() => (int)OptionGroupSingleton<UniversalModifierOptions>.Instance.ImmovableAmount;
    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && !(GameOptionsManager.Instance.currentNormalGameOptions.MapId is 4 or 6);
    }

    public Vector3 Location { get; set; } = Vector3.zero;

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Player.HasDied() || !Player.CanMove) return;

        Location = Player.transform.localPosition;
    }

    public void OnRoundStart()
    {
        if (Player.HasDied()) return;

        Player.transform.localPosition = Location;
        Player.NetTransform.SnapTo(Location);

        if (ModCompatibility.IsSubmerged())
        {
            ModCompatibility.ChangeFloor(Player.GetTruePosition().y > -7);
            ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
        }
    }
    public string GetAdvancedDescription()
    {
        return
            "You cannot be teleported to the meeting area, and you cannot get dispersed or teleported.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];
}
