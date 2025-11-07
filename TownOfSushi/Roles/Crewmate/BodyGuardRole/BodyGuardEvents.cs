using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;

namespace TownOfSushi.Roles.Crewmate;

public static class BodyGuardEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        var BodyGuardForts = ModifierUtils.GetActiveModifiers<BodyGuardGuardedModifier>();

        if (!BodyGuardForts.Any())
        {
            return;
        }

        foreach (var mod in BodyGuardForts)
        {
            var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
            var show = OptionGroupSingleton<BodyGuardOptions>.Instance.ShowGuarded;

            var showShieldedEveryone = show == BGProtectOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == mod.Player.PlayerId &&
                                   show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyGuard;
            var showShieldedBodyGuard = PlayerControl.LocalPlayer.PlayerId == mod.BodyGuard.PlayerId &&
                                     show is BGProtectOptions.BodyGuard or BGProtectOptions.SelfAndBodyGuard;

            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
                x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
            var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x =>
                x.PlayerId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);

            mod.ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedBodyGuard || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body);
        }
    }

    [RegisterEvent(-1)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }
        // Adding Warlock's button to check because:
        // Warlock's curse button somehow breaks when interacting with BodyGuard's protectee so this is the easiest way to fix it
        if (button is IKillButton || button == CustomButtonSingleton<WarlockCurseButton>.Instance 
        || button == CustomButtonSingleton<PoisonerButton>.Instance)
        {
            CheckForBodyGuardProtectMurder(@event, source, target);
        }
        else
        {
            CheckForBodyGuardProtect(@event, source, target);
        }
    }

    [RegisterEvent(-1)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        CheckForBodyGuardProtect(@event, source, target);
        CheckForBodyGuardProtectMurder(@event, source, target);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var victim = @event.Target;

        foreach (var BodyGuard in CustomRoleUtils.GetActiveRolesOfType<BodyGuardRole>())
        {
            if (victim == BodyGuard.Guarded)
            {
                BodyGuard.Clear();
            }
        }
    }

    private static void CheckForBodyGuardProtect(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<BodyGuardGuardedModifier>() || source == target ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return;
        }

        @event.Cancel();

        var BodyGuard = target.GetModifier<BodyGuardGuardedModifier>()?.BodyGuard.GetRole<BodyGuardRole>();

        if (BodyGuard != null && source.AmOwner)
        {
            BodyGuardRole.RpcBodyGuardNotify(BodyGuard.Player, source, target);
        }
    }
    private static void CheckForBodyGuardProtectMurder(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (!target.HasModifier<BodyGuardGuardedModifier>() || source == target || MeetingHud.Instance || (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield)) return;
        @event.Cancel();

        var BodyGuard = target.GetModifier<BodyGuardGuardedModifier>()?.BodyGuard.GetRole<BodyGuardRole>();

        if (BodyGuard != null && source.AmOwner && !BodyGuard.Player.HasDied())
        {
            BodyGuardRole.RpcBodyGuardGuardMurder(BodyGuard.Player, target, source);
        }
    }
}