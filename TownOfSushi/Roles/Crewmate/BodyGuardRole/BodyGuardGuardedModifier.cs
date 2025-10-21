using MiraAPI.Events;
using MiraAPI.LocalSettings;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;
using UnityEngine;

namespace TownOfSushi.Roles.Crewmate;

public sealed class BodyguardGuardedModifier(PlayerControl Bodyguard) : BaseShieldModifier
{
    public override string ModifierName => "Guarded";
    public override LoadableAsset<Sprite>? ModifierIcon => TOSRoleIcons.Bodyguard;
    public override string ShieldDescription => "You are protected by a Bodyguard!\nNo one can interact with you.";
    public bool ShowFort { get; set; }

    public override bool HideOnUi
    {
        get
        {
            var showFort = OptionGroupSingleton<BodyguardOptions>.Instance.ShowGuarded;
            return !LocalSettingsTabSingleton<TownOfSushiLocalSettings>.Instance.ShowShieldHudToggle.Value || showFort is BGProtectOptions.Bodyguard;
        }
    }

    public override bool VisibleSymbol
    {
        get
        {
            var show = OptionGroupSingleton<BodyguardOptions>.Instance.ShowGuarded;
            var showShieldedEveryone = show == BGProtectOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                                   show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyguard;
            return showShieldedSelf || showShieldedEveryone;
        }
    }

    public PlayerControl Bodyguard { get; } = Bodyguard;

    public override void OnActivate()
    {
        base.OnActivate();
        var TOSAbilityEvent = new TOSAbilityEvent(AbilityType.BodyguardProtect, Bodyguard, Player);
        MiraEventManager.InvokeEvent(TOSAbilityEvent);
        
        var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
        var show = OptionGroupSingleton<BodyguardOptions>.Instance.ShowGuarded;

        var showShieldedEveryone = show == BGProtectOptions.Everyone;
        var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == Player.PlayerId &&
                               show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyguard;
        var showShieldedBodyguard = PlayerControl.LocalPlayer.PlayerId == Bodyguard.PlayerId &&
                                 show is BGProtectOptions.Bodyguard or BGProtectOptions.SelfAndBodyguard;

        var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
            x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x =>
            x.PlayerId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
        
        ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedBodyguard || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body);
    }
}