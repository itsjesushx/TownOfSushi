using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class EclipsalBlindButton : TownOfSushiRoleButton<EclipsalRole>, IAftermathableButton
{
    public override string Name => "Blind";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<EclipsalOptions>.Instance.BlindCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<EclipsalOptions>.Instance.BlindDuration;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.BlindSprite;

    protected override void OnClick()
    {
        OverrideName("Unblinding");
        var blindRadius = OptionGroupSingleton<EclipsalOptions>.Instance.BlindRadius;
        var blindedPlayers =
            Helpers.GetClosestPlayers(PlayerControl.LocalPlayer, blindRadius * ShipStatus.Instance.MaxLightRadius);

        foreach (var player in blindedPlayers.Where(x => !x.HasDied() && !x.IsImpostor()))
        {
            player.RpcAddModifier<EclipsalBlindModifier>(PlayerControl.LocalPlayer);
        }
        // PlayerControl.LocalPlayer.RpcAddModifier<EclipsalBlindModifier>(PlayerControl.LocalPlayer);
    }

    public override void OnEffectEnd()
    {
        OverrideName("Blind");
    }
}