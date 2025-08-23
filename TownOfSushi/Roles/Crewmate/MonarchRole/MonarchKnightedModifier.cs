using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class MonarchKnightedModifier(PlayerControl monarch) : BaseModifier
{
    public override string ModifierName => "Knighted";
    public override string GetDescription() => "You are knighted by a Monarch!\nYou have an extra vote in meetings!";
    public PlayerControl Monarch { get; } = monarch;
    public override void OnActivate()
    {
        base.OnActivate();

        var TosAbilityEvent = new TOSAbilityEvent(AbilityType.MonarchKnight, Monarch, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }
    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (Monarch.AmOwner)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Monarch));
        }
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Monarch));
        Player.RpcRemoveModifier<MonarchKnightedModifier>();
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Monarch));
    }
}