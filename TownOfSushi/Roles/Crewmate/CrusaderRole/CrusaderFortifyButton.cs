using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class CrusaderFortifyButton : TownOfSushiRoleButton<CrusaderRole, PlayerControl>
{
    public override string Name => "Fortify";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Crusader;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.FortifySprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Fortified: null };
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Crusader Fortify: Target is null");
            return;
        }

        CrusaderRole.RpcCrusaderFortify(PlayerControl.LocalPlayer, Target);
    }
}