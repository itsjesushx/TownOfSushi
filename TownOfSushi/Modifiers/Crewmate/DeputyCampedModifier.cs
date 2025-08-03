using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Utilities;
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

        var TosAbilityEvent = new TosAbilityEvent(AbilityType.DeputyCamp, Deputy, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
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
        if (Deputy.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.Deputy.ToTextColor()}Your camped target, {Player.Data.PlayerName}, has died! Avenge them in the meeting.</color></b>",
                Color.white, spr: TosRoleIcons.Deputy.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
            Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Deputy));
        }
    }

    public override void OnDeactivate()
    {
        Player.cosmetics.SetOutline(false, new Il2CppSystem.Nullable<Color>(TownOfSushiColors.Deputy));
    }
}
