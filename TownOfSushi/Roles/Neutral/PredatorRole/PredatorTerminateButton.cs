using MiraAPI.Hud;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public sealed class PredatorTerminateButton : TownOfSushiRoleButton<PredatorRole>
{
    public override string Name => "Terminate";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Predator;
    public override float Cooldown => OptionGroupSingleton<PredatorOptions>.Instance.TerminateCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<PredatorOptions>.Instance.TerminateDuration;
    public override LoadableAsset<Sprite> Sprite => TOSNeutAssets.TerminateSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role?.Terminating == false;
    }

    protected override void OnClick()
    {
        if (Role == null)
        {
            return;
        }

        Role.Terminating = true;

        CustomButtonSingleton<PredatorKillButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<PredatorKillButton>.Instance.SetTimer(0.01f);
        TOSAudio.PlaySound(TOSAudio.PredatorTerminateSound);
    }

    public override void OnEffectEnd()
    {
        if (Role == null)
        {
            return;
        }

        Role.Terminating = false;

        CustomButtonSingleton<PredatorKillButton>.Instance.SetActive(false, Role);
    }
}