using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TownOfSushi.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class HypnotistHypnotiseButton : TownOfSushiRoleButton<HypnotistRole, PlayerControl>, IAftermathablePlayerButton
{
    public override string Name => "Hypnotise";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<HypnotistOptions>.Instance.HypnotiseCooldown;
    public override LoadableAsset<Sprite> Sprite => TosImpAssets.HypnotiseButtonSprite;

    public override bool CanUse()
    {
        return base.CanUse() && !Role.HysteriaActive;
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }

        Target.RpcAddModifier<HypnotisedModifier>(PlayerControl.LocalPlayer);
    }

    public override PlayerControl? GetTarget() => PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, false, player => !player.HasModifier<HypnotisedModifier>());
}
