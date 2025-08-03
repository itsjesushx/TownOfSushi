using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Modifiers.Crewmate;

public sealed class DeputyCampedModifier(PlayerControl deputy) : BaseModifier
{
    public override string ModifierName => "Camped";
    public override bool HideOnUi => true;

    public PlayerControl Deputy { get; } = deputy;

    public override void OnActivate()
    {
        base.OnActivate();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.DeputyCamp, Deputy, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Deputy.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Deputy));
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Deputy));
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Deputy));
    }
}