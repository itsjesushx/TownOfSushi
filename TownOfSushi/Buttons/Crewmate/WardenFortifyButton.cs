using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class WardenFortifyButton : TownOfSushiRoleButton<WardenRole, PlayerControl>
{
    public override string Name => "Fortify";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Warden;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.FortifySprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Fortified: null };
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Warden Fortify: Target is null");
            return;
        }

        WardenRole.RpcWardenFortify(PlayerControl.LocalPlayer, Target);
    }
}
