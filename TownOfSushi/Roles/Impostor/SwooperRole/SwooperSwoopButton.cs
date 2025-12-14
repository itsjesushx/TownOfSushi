using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class SwooperSwoopButton : TownOfSushiRoleButton<SwooperRole>, IAftermathableButton
{
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override string Name => "Swoop";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override float Cooldown => OptionGroupSingleton<SwooperOptions>.Instance.SwoopCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<SwooperOptions>.Instance.SwoopDuration;
    public override int MaxUses => (int)OptionGroupSingleton<SwooperOptions>.Instance.MaxSwoops;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.SwoopSprite;

    public void AftermathHandler()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<SwoopModifier>();
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    public override bool CanUse()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() || PlayerControl.LocalPlayer
                .GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return ((Timer <= 0 && !EffectActive && (MaxUses == 0 || UsesLeft > 0)) ||
                (EffectActive && Timer <= EffectDuration - 2f));
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<SwoopModifier>();
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            OnEffectEnd();
        }
    }

    public override void OnEffectEnd()
    {
        if (!PlayerControl.LocalPlayer.HasModifier<SwoopModifier>())
        {
            return;
        }

        PlayerControl.LocalPlayer.RpcRemoveModifier<SwoopModifier>();
    }
}