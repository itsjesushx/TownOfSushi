using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles.Crewmate;
using UnityEngine;

namespace TownOfSushi.Buttons.Crewmate;

public sealed class EngineerFixButton : TownOfSushiRoleButton<EngineerTouRole>
{
    public override string Name => "Fix";
    public override string Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Engineer;
    public override float Cooldown => 0.001f + MapCooldown;
    public override float EffectDuration => OptionGroupSingleton<EngineerOptions>.Instance.FixDelay + 0.01f;
    public override int MaxUses => (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxFixes;
    public override LoadableAsset<Sprite> Sprite => TosCrewAssets.FixButtonSprite;

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        Button?.cooldownTimerText.gameObject.SetActive(false);
    }
    public override bool CanUse()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        return base.CanUse() && system is { AnyActive: true };
    }
    protected override void OnClick()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is not { AnyActive: true })
        {
            ResetCooldownAndOrEffect();
        }
    }
    public override void OnEffectEnd()
    {
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is { AnyActive: true })
        {
            List<LoadableAsset<AudioClip>> audio = [TosAudio.EngiFix1, TosAudio.EngiFix2, TosAudio.EngiFix3];
            TosAudio.PlaySound(audio.Random()!, 4f);
            EngineerTouRole.EngineerFix(PlayerControl.LocalPlayer);
        }
    }
}
