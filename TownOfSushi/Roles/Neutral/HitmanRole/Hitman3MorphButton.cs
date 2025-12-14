using MiraAPI.Hud;
using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class HitmanMorphButton : TownOfSushiRoleButton<HitmanRole>, IAftermathableButton
{
    public override string Name => "Morph";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Hitman;
    public override float Cooldown => OptionGroupSingleton<AgentOptions>.Instance.MorphCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<AgentOptions>.Instance.MorphDuration;
    public override LoadableAsset<Sprite> Sprite => TOSImpAssets.MorphSprite;
    public override ButtonLocation Location => ButtonLocation.BottomRight;
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
        return role is HitmanRole;
    }

    public void AftermathHandler()
    {
        if (!EffectActive)
        {
            var player = PlayerControl.AllPlayerControls.ToArray().Where(plr => (!plr.HasDied() ||
                UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId) && plr != PlayerControl.LocalPlayer)).Random();
            if (player != null)
            {
                TOSAudio.PlaySound(TOSAudio.MimicSound);
                PlayerControl.LocalPlayer.RpcAddModifier<HitmanMorphModifier>(player);

                EffectActive = true;
                Timer = EffectDuration;
                OverrideName("UnMorph");
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<HitmanMorphModifier>();
            OverrideName("Morph");
            TOSAudio.PlaySound(TOSAudio.UnmimicSound);
        }
    }

    protected override void OnClick()
    {
        if (!EffectActive)
        {
       /*     if (!OptionGroupSingleton<AgentOptions>.Instance.MoveWithMenu)
            {
                PlayerControl.LocalPlayer.NetTransform.Halt();
            }*/

            var playerMenu = CustomPlayerMenu.Create();
            playerMenu.transform.FindChild("PhoneUI").GetChild(0).GetComponent<SpriteRenderer>().material =
                PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.transform.FindChild("PhoneUI").GetChild(1).GetComponent<SpriteRenderer>().material =
                PlayerControl.LocalPlayer.cosmetics.currentBodySprite.BodySprite.material;
            playerMenu.Begin(
                plr => (!plr.HasDied() ||
                        UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == plr.PlayerId)) && plr != PlayerControl.LocalPlayer,
                plr =>
                {
                    playerMenu.ForceClose();

                    if (plr != null)
                    {
                        TOSAudio.PlaySound(TOSAudio.MimicSound);
                        PlayerControl.LocalPlayer.RpcAddModifier<HitmanMorphModifier>(plr);

                        EffectActive = true;
                        Timer = EffectDuration;
                        OverrideName("UnMorph");
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
            PlayerControl.LocalPlayer.RpcRemoveModifier<HitmanMorphModifier>();
            OverrideName("Morph");
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
        OverrideName("Morph");
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