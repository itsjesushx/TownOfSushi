using Reactor.Utilities.Extensions;
using TownOfSushi.Buttons;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class EngineerFixButton : TownOfSushiRoleButton<EngineerTOSRole>
{
    public override string Name => "Fix";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfSushiColors.Engineer;
    public override float Cooldown => 0.001f;
    public override float EffectDuration => OptionGroupSingleton<EngineerOptions>.Instance.FixDelay + 0.01f;
    public override int MaxUses => (int)OptionGroupSingleton<EngineerOptions>.Instance.MaxFixes;
    public override LoadableAsset<Sprite> Sprite => TownOfSushiAssets.FixButtonSprite;
    public override bool ShouldPauseInVent => false;

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
        OverrideName("Fixing");
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is not { AnyActive: true })
        {
            ResetCooldownAndOrEffect();
        }
    }

    public override void OnEffectEnd()
    {
        OverrideName("Fix");
        var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

        if (system is { AnyActive: true })
        {
            List<LoadableAsset<AudioClip>> audio = [TownOfSushiAudio.EngiFix1, TownOfSushiAudio.EngiFix2, TownOfSushiAudio.EngiFix3];
            TownOfSushiAudio.PlaySound(audio.Random()!, 4f);
            EngineerTOSRole.EngineerFix(PlayerControl.LocalPlayer);
        }
    }
}