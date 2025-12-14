using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class SeeButton : TownOfSushiRoleButton<LookoutRole>
{
    public override string Name => "See";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Lookout;
    public override float Cooldown => OptionGroupSingleton<LookoutOptions>.Instance.WatchCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<LookoutOptions>.Instance.WatchDuration + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.Observe;
    public override bool CanUse()
    {
        return base.CanUse() && Role is not { ObservedPlayer: null };
    }
    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is not { ObservedPlayer: null };
    }
    protected override void OnClick()
    {
        if (Role.ObservedPlayer == null)
        {
            return;
        }

        LookoutRole.RpcSeePlayer(Role.ObservedPlayer, PlayerControl.LocalPlayer);
    }
    public override void OnEffectEnd()
    {
        if (Role.ObservedPlayer == null)
        {
            return;
        }

        LookoutRole.RpcUnSeePlayer(Role.ObservedPlayer, PlayerControl.LocalPlayer);
    }
}