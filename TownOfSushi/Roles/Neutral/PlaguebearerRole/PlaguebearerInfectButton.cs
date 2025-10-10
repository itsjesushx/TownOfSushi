using Reactor.Utilities;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PlaguebearerInfectButton : TownOfSushiRoleButton<PlaguebearerRole, PlayerControl>
{
    public override string Name => "Infect";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Plaguebearer;
    public override float Cooldown => OptionGroupSingleton<PlaguebearerOptions>.Instance.InfectCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.InfectSprite;

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance,
            predicate: plr => !plr.HasModifier<PlaguebearerInfectedModifier>());
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Logger<TownOfSushiPlugin>.Error("Plaguebearer Infect: Target is null");
            return;
        }

        PlaguebearerRole.RpcCheckInfected(PlayerControl.LocalPlayer, Target);
    }
}