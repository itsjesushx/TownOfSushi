using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyguardGuardButton : TownOfSushiRoleButton<BodyguardRole, PlayerControl>
{
    public override string Name => "Guard";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Bodyguard;
    public override float Cooldown => 0.001f + MapCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSCrewAssets.GuardSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role is { Guarded: null };
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Bodyguard Guard: Target is null");
            return;
        }

        BodyguardRole.RpcBodyguardGuard(PlayerControl.LocalPlayer, Target);
    }
}