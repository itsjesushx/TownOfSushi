using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Utilities.Appearances;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor.Venerer;

public sealed class VenererCamouflageModifier : ConcealedModifier, IVenererModifier, IVisualAppearance
{
    public override string ModifierName => "Camouflaged";
    public override float Duration => OptionGroupSingleton<VenererOptions>.Instance.AbilityDuration;
    public override bool AutoStart => true;
    public bool VisualPriority => true;
    public override bool VisibleToOthers => true;

    public VisualAppearance GetVisualAppearance()
    {
        var appearance = Player.GetDefaultAppearance();
        appearance.Speed = 1f;
        appearance.Size = new Vector3(0.7f, 0.7f, 1f);
        appearance.ColorId = Player.Data.DefaultOutfit.ColorId;
        appearance.HatId = string.Empty;
        appearance.SkinId = string.Empty;
        appearance.VisorId = string.Empty;
        appearance.PlayerName = string.Empty;
        appearance.PetId = string.Empty;
        appearance.NameVisible = false;
        appearance.PlayerMaterialColor = Color.grey;
        return appearance;
    }

    public override void OnActivate()
    {
        Player.RawSetAppearance(this);

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.VenererCamoAbility, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeactivate()
    {
        Player?.ResetAppearance();
    }
}