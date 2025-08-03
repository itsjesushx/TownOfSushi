using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Options.Roles.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Impostor;

public sealed class HypnotistHypnotizeButton : TownOfSushiRoleButton<HypnotistRole, PlayerControl>,
    IAftermathablePlayerButton
{
    public override string Name => "Hypnotize";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<HypnotistOptions>.Instance.HypnotiseCooldown;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.HypnotiseButtonSprite;

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && role is HypnotistRole hypno && !hypno.HysteriaActive;
    }

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

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, false,
            player => !player.HasModifier<HypnotisedModifier>());
    }
}