using MiraAPI.Hud;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfSushi.Roles.Neutral;

public sealed class GlitchMimicButton : TownOfSushiRoleButton<GlitchRole>, IAftermathableButton
{
    public override string Name => "Mimic";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Glitch;
    public override float Cooldown => OptionGroupSingleton<GlitchOptions>.Instance.MimicCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<GlitchOptions>.Instance.MimicDuration;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.MimicSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
    public override bool ShouldPauseInVent => false;

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
        return role is GlitchRole;
    }

    public void AftermathHandler()
    {
        if (!EffectActive)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().Where(plr => (!plr.HasDied() ||
                Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) && plr != PlayerControl.LocalPlayer)).Random();
            if (player != null)
            {
                TOSAudio.PlaySound(TOSAudio.MimicSound);
                PlayerControl.LocalPlayer.RpcAddModifier<GlitchMimicModifier>(player);

                EffectActive = true;
                Timer = EffectDuration;
                OverrideName("Unmimic");
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<GlitchMimicModifier>();
            OverrideName("Mimic");
            TOSAudio.PlaySound(TOSAudio.UnmimicSound);
        }
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
            if (!OptionGroupSingleton<GlitchOptions>.Instance.MoveWithMenu)
            {
                PlayerControl.LocalPlayer.NetTransform.Halt();
            }

            var playerMenu = CustomPlayerMenu.Create();
            playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
                PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
                PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.Begin(
                plr => (!plr.HasDied() ||
                        Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId)) && plr != PlayerControl.LocalPlayer,
                plr =>
                {
                    playerMenu.ForceClose();

                    if (plr != null)
                    {
                        TOSAudio.PlaySound(TOSAudio.MimicSound);
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
                if (panel.NameText.text != PlayerControl.LocalPlayer.Data.PlayerName)
                {
                    panel.NameText.color = Color.white;
                }
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<GlitchMimicModifier>();
            OverrideName("Mimic");
            if (MeetingHud.Instance == null)
            {
                TOSAudio.PlaySound(TOSAudio.UnmimicSound);
            }
        }
    }

    public override void OnEffectEnd()
    {
        if (MeetingHud.Instance == null)
        {
            TOSAudio.PlaySound(TOSAudio.UnmimicSound);
        }
        OverrideName("Mimic");
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

        return ((Timer <= 0 && !EffectActive) || (EffectActive && Timer <= EffectDuration - 2f));
    }
}