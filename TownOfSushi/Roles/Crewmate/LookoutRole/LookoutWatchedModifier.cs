using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class LookoutWatchedModifier(PlayerControl lookout) : BaseModifier
{
    public override string ModifierName => "Watched";
    public override bool HideOnUi => true;
    public PlayerControl Lookout { get; set; } = lookout;

    public override void OnActivate()
    {
        base.OnActivate();

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.LookoutWatch, Lookout, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent!.RemoveModifier(this);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Lookout.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Lookout));
        }
    }
}