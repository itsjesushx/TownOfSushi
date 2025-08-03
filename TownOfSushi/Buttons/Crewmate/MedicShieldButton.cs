using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class MedicShieldButton : TownOfSushiRoleButton<MedicRole, PlayerControl>
{
    public bool CanChangeTarget = OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget;
    public override string Name => "Shield";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Medic;
    public override int MaxUses => OptionGroupSingleton<MedicOptions>.Instance.ChangeTarget ? (int)OptionGroupSingleton<MedicOptions>.Instance.MedicShieldUses : 0;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.MedicSprite;

    public override bool CanUse()
    {
        return base.CanUse() && (Role is { Shielded: null } || CanChangeTarget);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Medic Shield: Target is null");
            return;
        }

        MedicRole.RpcMedicShield(PlayerControl.LocalPlayer, Target);
        CanChangeTarget = false;
    }
}