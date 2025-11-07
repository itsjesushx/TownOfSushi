using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Universal;

public sealed class LazyModifier : UniversalGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Lazy";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSModifierIcons.Lazy;

    public override ModifierFaction FactionType => ModifierFaction.UniversalPassive;
    public override Color FreeplayFileColor => new Color32(180, 180, 180, 255);

    public Vector3 Location { get; set; } = Vector3.zero;

    public string GetAdvancedDescription()
    {
        return
            "You cannot be teleported to the meeting area, and you cannot get dispersed or teleported.";
    }

    public List<CustomButtonWikiDescription> Abilities { get; } = [];

    public override string GetDescription()
    {
        return "You are unable to be moved via abilities and meetings.";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<LazyOptions>.Instance.LazyChance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<LazyOptions>.Instance.LazyAmount;
    }

    public override bool IsModifierValidOn(RoleBehaviour role)
    {
        return base.IsModifierValidOn(role) && !(GameOptionsManager.Instance.currentNormalGameOptions.MapId is 4 or 6);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Player.HasDied() || !Player.CanMove)
        {
            return;
        }

        Location = Player.transform.localPosition;
    }

    public void OnRoundStart()
    {
        if (Player.HasDied())
        {
            return;
        }

        Player.transform.localPosition = Location;
        Player.NetTransform.SnapTo(Location);

        if (ModCompatibility.IsSubmerged())
        {
            ModCompatibility.ChangeFloor(Player.GetTruePosition().y > -7);
            ModCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
        }
    }
}