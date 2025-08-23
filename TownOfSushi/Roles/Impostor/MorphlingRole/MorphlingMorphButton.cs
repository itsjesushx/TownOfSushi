﻿using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Impostor;

public sealed class MorphlingMorphButton : TownOfSushiRoleButton<MorphlingRole>, IAftermathableButton
{
    public override string Name => "Morph";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Impostor;
    public override float Cooldown => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<MorphlingOptions>.Instance.MorphlingDuration;
    public override int MaxUses => (int)OptionGroupSingleton<MorphlingOptions>.Instance.MaxMorphs;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.MorphSprite;

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

    public override bool Enabled(RoleBehaviour? role)
    {
        return base.Enabled(role) && Role is not { Sampled: null };
    }

    public override bool CanUse()
    {
        return ((Timer <= 0 && !EffectActive) || (EffectActive && Timer <= EffectDuration - 2f)) &&
               !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() &&
               !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>();
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            PlayerControl.LocalPlayer.RpcAddModifier<MorphlingMorphModifier>(Role.Sampled!);
            OverrideName("Unmorph");
            UsesLeft--;
            if (MaxUses != 0)
            {
                Button?.SetUsesRemaining(UsesLeft);
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<MorphlingMorphModifier>();
            OverrideName("Morph");
        }
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        PlayerControl.LocalPlayer.RpcRemoveModifier<MorphlingMorphModifier>();
        OverrideName("Morph");
    }
}