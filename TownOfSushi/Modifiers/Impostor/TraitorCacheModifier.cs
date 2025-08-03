using AmongUs.GameOptions;
using MiraAPI.Events;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using TownOfSushi.Events.TosEvents;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Modifiers.Impostor;

public sealed class TraitorCacheModifier : BaseModifier, ICachedRole
{
    public override string ModifierName => "Traitor";
    public override bool HideOnUi => true;
    public bool ShowCurrentRoleFirst => true;
    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() || ModdedGuardianAngelRole.GASeesRoleVisibilityFlag(Player);
    public RoleBehaviour CachedRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TraitorRole>());

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }
    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(
                $"<b>{TownOfSushiColors.ImpSoft.ToTextColor()}You are a new role, and you are only guessable as Traitor now!</color></b>", Color.white, spr: TosRoleIcons.Traitor.LoadAsset());

            notif1.Text.SetOutlineThickness(0.35f);
            notif1.transform.localPosition = new Vector3(0f, 1f, -20f);
        }
        var TosAbilityEvent = new TosAbilityEvent(AbilityType.TraitorChangeRole, Player);
        MiraEventManager.InvokeEvent(TosAbilityEvent);
    }

    public override void OnDeactivate()
    {
        if (Player.IsRole<TraitorRole>()) return;

        Player.RpcChangeRole(RoleId.Get<TraitorRole>(), false);
    }
}
