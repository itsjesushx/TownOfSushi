using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
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
    public override bool Enabled(RoleBehaviour? role) => role is HitmanRole;

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
            }
        }
        else
        {
            PlayerControl.LocalPlayer.RpcRemoveModifier<HitmanMorphModifier>();
            OverrideName("Morph");
            TOSAudio.PlaySound(TOSAudio.UnmimicSound);
        }
    }

    public override void OnEffectEnd()
    {
        TOSAudio.PlaySound(TOSAudio.UnmimicSound);
        OverrideName("Morph");
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