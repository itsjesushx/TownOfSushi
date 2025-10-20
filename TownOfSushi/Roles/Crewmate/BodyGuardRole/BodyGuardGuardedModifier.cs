using MiraAPI.Events;
using MiraAPI.LocalSettings;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyGuardGuardedModifier(PlayerControl BodyGuard) : BaseShieldModifier
{
    public override string ModifierName => "Guarded";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.BodyGuard;
    public override string ShieldDescription => "You are protected by a BodyGuard!\nNo one can interact with you.";
    public bool ShowFort { get; set; }

    public override bool HideOnUi
    {
        get
        {
            var showFort = OptionGroupSingleton<BodyGuardOptions>.Instance.ShowGuarded;
            return !LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value || showFort is BGProtectOptions.BodyGuard;
        }
    }

    public override bool VisibleSymbol
    {
        get
        {
            var show = OptionGroupSingleton<BodyGuardOptions>.Instance.ShowGuarded;
            var showShieldedEveryone = show == BGProtectOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                                   show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyGuard;
            return showShieldedSelf || showShieldedEveryone;
        }
    }

    public PlayerControl BodyGuard { get; } = BodyGuard;

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.BodyGuardProtect, BodyGuard, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var show = OptionGroupSingleton<BodyGuardOptions>.Instance.ShowGuarded;

        var showShieldedEveryone = show == BGProtectOptions.Everyone;
        var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                               show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyGuard;
        var showShieldedBodyGuard = PlayerControl.LocalPlayer.PlayerId == BodyGuard.PlayerId &&
                                 show is BGProtectOptions.BodyGuard or BGProtectOptions.SelfAndBodyGuard;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x =>
            x.PlayerId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedBodyGuard || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body);
    }
}