using MiraAPI.Events;
using MiraAPI.Modifiers.Types;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class HunterStalkedModifier(PlayerControl hunter) : TimedModifier
{
    public override string ModifierName => "Stalked";
    public override bool HideOnUi => true;
    public override float Duration => OptionGroupSingleton<HunterOptions>.Instance.HunterStalkDuration;
    public PlayerControl Hunter { get; set; } = hunter;

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.HunterStalk, Hunter, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (PlayerControl.LocalPlayer.Data.Role is HunterRole)
        {
            Player?.cosmetics.SetOutline(true, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Hunter));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Hunter));
    }

    public override void OnDeath(DeathReason reason)
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Hunter));
    }
}