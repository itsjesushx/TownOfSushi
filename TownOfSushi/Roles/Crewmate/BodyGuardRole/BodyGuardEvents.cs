using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Game.Killer;
using TownOfSushi.Options;

namespace TownOfSushi.Roles.Crewmate;

public static class BodyguardEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        var BodyguardForts = ModifierUtils.GetActiveModifiers<BodyguardGuardedModifier>();

        if (!BodyguardForts.Any())
        {
            return;
        }

        foreach (var mod in BodyguardForts)
        {
            var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
            var show = OptionGroupSingleton<BodyguardOptions>.Instance.ShowGuarded;

            var showShieldedEveryone = show == BGProtectOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == mod.Player.PlayerId &&
                                   show is BGProtectOptions.Self or BGProtectOptions.SelfAndBodyguard;
            var showShieldedBodyguard = PlayerControl.LocalPlayer.PlayerId == mod.Bodyguard.PlayerId &&
                                     show is BGProtectOptions.Bodyguard or BGProtectOptions.SelfAndBodyguard;

            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
                x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);

            mod.ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedBodyguard || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body);
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
        // Warlock's curse button somehow breaks when interacting with Bodyguard's protectee so this is the easiest way to fix it
        if (button is IKillButton || button == CustomButtonSingleton<WarlockCurseButton>.Instance 
        || button == CustomButtonSingleton<PoisonerButton>.Instance)
        {
            CheckForBodyguardProtectMurder(@event, source, target);
        }
        else
        {
            CheckForBodyguardProtect(@event, source, target);
        }
    }

    [RegisterEvent(-1)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        CheckForBodyguardProtect(@event, source, target);
        CheckForBodyguardProtectMurder(@event, source, target);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var victim = @event.Target;

        foreach (var Bodyguard in CustomRoleUtils.GetActiveRolesOfType<BodyguardRole>())
        {
            if (victim == Bodyguard.Guarded)
            {
                Bodyguard.Clear();
            }
        }
    }

    private static void CheckForBodyguardProtect(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<BodyguardGuardedModifier>() || source == target ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return;
        }

        if (source.HasModifier<RuthlessModifier>())
        {
            return;
        }

        @event.Cancel();

        var Bodyguard = target.GetModifier<BodyguardGuardedModifier>()?.Bodyguard.GetRole<BodyguardRole>();

        if (Bodyguard != null && source.AmOwner)
        {
            BodyguardRole.RpcBodyguardNotify(Bodyguard.Player, source, target);
        }
    }
    private static void CheckForBodyguardProtectMurder(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (!target.HasModifier<BodyguardGuardedModifier>() || source == target || MeetingHud.Instance || (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield)) return;
        @event.Cancel();

        if (source.HasModifier<RuthlessModifier>())
        {
            return;
        }

        var Bodyguard = target.GetModifier<BodyguardGuardedModifier>()?.Bodyguard.GetRole<BodyguardRole>();

        if (Bodyguard != null && source.AmOwner && !Bodyguard.Player.HasDied())
        {
            BodyguardRole.RpcBodyguardGuardMurder(Bodyguard.Player, target, source);
        }
    }
}