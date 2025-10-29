using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class PoisonerButton : TownOfSushiRoleButton<PoisonerRole, PlayerControl>
{
    public override string Name => "Poison";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<PoisonerOptions>.Instance.PoisonCooldown;
    public override float EffectDuration => OptionGroupSingleton<PoisonerOptions>.Instance.PoisonDelay;
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
            PoisonerRole.RpcSetPoisoned(PlayerControl.LocalPlayer, Poisoned);
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

        PoisonerRole.RpcMurderPoisoned(PlayerControl.LocalPlayer, Poisoned);
        OverrideName("Poison");
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance, false);
    }
}