﻿using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class GlitchMimicButton : TownOfSushiRoleButton<GlitchRole>, IAftermathableButton
{
    public override string Name => "Mimic";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.MimicCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GlitchOptions>.Instance.MimicDuration;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.MimicSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool Enabled(RoleBehaviour? role) => role is GlitchRole;

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            var playerMenu = CustomPlayerMenu.Create();
            playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material = PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.Begin(
                plr => (!plr.HasDied() || UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) ||
                FakePlayer.FakePlayers.FirstOrDefault(x => x?.body?.name == $"Fake {plr.gameObject.name}")?.body) && plr != PlayerControl.LocalPlayer,
                plr =>
                {
                    playerMenu.ForceClose();

                    if (plr != null)
                    {
                        TosAudio.PlaySound(TosAudio.MimicSound);
                        PlayerControl.LocalPlayer.RpcAddModifier<GlitchMimicModifier>(plr);

                        EffectActive = true;
                        Timer = EffectDuration;
                        OverrideName("Unmimic");
                    }
                    else
                    {
                        EffectActive = false;
                        Timer = 0.01f;
                    }
                });
            foreach (var panel in playerMenu.potentialVictims)
            {
                panel.PlayerIcon.cosmetics.SetPhantomRoleAlpha(1f);
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<GlitchMimicModifier>();
            OverrideName("Mimic");
            TosAudio.PlaySound(TosAudio.UnmimicSound);
        }
    }

    public override void OnEffectEnd()
    {
        TosAudio.PlaySound(TosAudio.UnmimicSound);
        OverrideName("Mimic");
    }

    public override bool CanUse()
    {
        return ((Timer <= 0 && !EffectActive) || (EffectActive && Timer <= (EffectDuration - 2f))) && !PlayerControl.LocalPlayer.HasModifier<GlitchHackedModifier>() && !PlayerControl.LocalPlayer.HasModifier<DisabledModifier>();
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
}
