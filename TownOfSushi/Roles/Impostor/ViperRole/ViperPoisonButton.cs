using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class ViperButton : TownOfSushiRoleButton<ViperRole, PlayerControl>
{
    public override string Name => "Poison";
    public override string Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<ViperOptions>.Instance.PoisonCooldown;
    public override float EffectDuration => OptionGroupSingleton<ViperOptions>.Instance.PoisonDelay;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.PoisonSprite;
    public PlayerControl? Poisoned { get; set; }
    protected override void OnClick()
    {
        if (Target == null)
        {
            return;
        }
        if (!EffectActive)
        {
            Poisoned = Target;
            ViperRole.RpcSetPoisoned(PlayerControl.LocalPlayer, Poisoned);
            OverrideName("Poisoned");
        }
        else
        {
            OnEffectEnd();
        }
    }
    public override void OnEffectEnd()
    {
        if (Poisoned == null || Poisoned.Data.IsDead)
        {
            return;
        }

        ViperRole.RpcMurderPoisoned(PlayerControl.LocalPlayer, Poisoned);
        OverrideName("Poison");
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false);
    }
}