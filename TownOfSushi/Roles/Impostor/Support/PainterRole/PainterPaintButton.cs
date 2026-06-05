using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class PainterPaintButton : TownOfSushiRoleButton<PainterRole>, IAftermathableButton
{
    public override string Name => "Paint";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<PainterOptions>.Instance.PaintCooldown;
    public override float EffectDuration => OptionGroupSingleton<PainterOptions>.Instance.PaintDuration;
    public override int MaxUses => (int)OptionGroupSingleton<PainterOptions>.Instance.MaxPaints;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.PaintSprite;
    public override bool CanUse()
    {
        return base.CanUse() && !Utils.MushroomSabotageActive();
    }
    public void AftermathHandler()
    {
        if (!EffectActive)
        {
            TownOfSushiAudio.PlaySound(TownOfSushiAudio.MimicSound);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.HasDied())
                {
                    continue;
                }
                player.RpcAddModifier<PainterPaintedModifier>();
            }

            EffectActive = true;
            Timer = EffectDuration;
            OverrideName("UnPaint");
        }
        else
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.HasDied())
                {
                    continue;
                }
                player.RpcRemoveModifier<PainterPaintedModifier>();
            }
            OverrideName("Paint");
            TownOfSushiAudio.PlaySound(TownOfSushiAudio.UnmimicSound);
        }
    }
    protected override void OnClick()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                continue;
            }
            player.RpcAddModifier<PainterPaintedModifier>();
            TownOfSushiAudio.PlaySound(TownOfSushiAudio.MimicSound);
        }
        OverrideName("UnPaint");
    }

    public override void OnEffectEnd()
    {
        foreach (var player in PlayerControl.AllPlayerControls)
        {
            if (player.HasDied())
            {
                continue;
            }
            player.RpcRemoveModifier<PainterPaintedModifier>();
            TownOfSushiAudio.PlaySound(TownOfSushiAudio.UnmimicSound);
        }
        OverrideName("Paint");
    }
}