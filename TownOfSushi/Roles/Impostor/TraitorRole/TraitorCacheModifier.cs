using AmongUs.GameOptions;
using MiraAPI.Events;
using TownOfSushi.Events.TOSEvents;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class TraitorCacheModifier : BaseModifier, ICachedRole
{
    public override string ModifierName => "Traitor";
    public override bool HideOnUi => true;
    public bool ShowCurrentRoleFirst => true;

    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() ||
                           GuardianAngelTOSRole.GASeesRoleVisibilityFlag(Player) || RomanticRole.RomamticSeesRoleVisibilityFlag(Player);

    public RoleBehaviour CachedRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<TraitorRole>());

    public override void OnDeath(DeathReason reason)
    {
        ModifierComponent?.RemoveModifier(this);
    }

    public override void OnActivate()
    {
        if (Player.AmOwner)
        {
            var notif1 = Helpers.CreateAndShowNotification(MiscUtils.ColorString(TownOfSushiColors.ImpSoft,
                $"<b>You are a new role, and you are only guessable as Traitor now!</b>"),
                Color.white, spr: TOSRoleIcons.Traitor.LoadAsset());

            
            notif1.AdjustNotification();
        }

        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.TraitorChangeRole, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
    }

    public override void OnDeactivate()
    {
        if (Player.IsRole<TraitorRole>())
        {
            return;
        }

        Player.RpcChangeRole(RoleId.Get<TraitorRole>(), false);
    }
}