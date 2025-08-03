using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Utilities.Assets;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;
using UnityEngine;

namespace TownOfSushi.Buttons.Neutral;

public sealed class WerewolfRampageButton : TownOfSushiRoleButton<PredatorRole>, IAftermathableButton
{
    public override string Name => "Terminate";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Predator;
    public override float Cooldown => OptionGroupSingleton<PredatorOptions>.Instance.TerminateCooldown + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<PredatorOptions>.Instance.TerminateDuration;
    public override LoadableAsset<Sprite> Sprite => TosNeutAssets.TerminateSprite;

    public override bool CanUse()
    {
        return base.CanUse() && Role?.Rampaging == false;
    }

    protected override void OnClick()
    {
        if (Role == null) return;

        Role.Rampaging = true;

        CustomButtonSingleton<PredatorKillButton>.Instance.SetActive(true, Role);
        CustomButtonSingleton<PredatorKillButton>.Instance.SetTimer(0.01f);
        TosAudio.PlaySound(TosAudio.WerewolfRampageSound);
    }

    public override void OnEffectEnd()
    {
        if (Role == null) return;

        Role.Rampaging = false;

        CustomButtonSingleton<PredatorKillButton>.Instance.SetActive(false, Role);
    }
}
