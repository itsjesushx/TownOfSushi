using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class ConsigliereRevealButton : TownOfSushiRoleButton<ConsigliereRole, PlayerControl>
{
    public override string Name => "Reveal";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<ConsigliereOptions>.Instance.RevealCooldown;
    public override float EffectDuration => OptionGroupSingleton<ConsigliereOptions>.Instance.RevealDuration;
    public override int MaxUses => (int)OptionGroupSingleton<ConsigliereOptions>.Instance.MaxConsiglieres;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.ConsigliereRevealSprite;

    protected override void OnClick() {   }
    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        if (Target == null)
        {
            return;
        }

        ConsigliereRole.RpcReveal(PlayerControl.LocalPlayer, Target);
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(false, Distance, false,
            player => !player.HasModifier<ConsigliereRevealedModifier>());
    }
}