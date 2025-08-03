using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class PlaguebearerInfectButton : TownOfSushiRoleButton<PlaguebearerRole, PlayerControl>
{
    public override string Name => "Infect";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Plaguebearer;
    public override float Cooldown => OptionGroupSingleton<PlaguebearerOptions>.Instance.InfectCooldown;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.InfectSprite;

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, predicate: plr => !plr.HasModifier<PlaguebearerInfectedModifier>());

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
