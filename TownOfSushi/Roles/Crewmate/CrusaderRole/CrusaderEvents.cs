using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using TownOfSushi.Options;

namespace TownOfSushi.Roles.Crewmate;

public static class CrusaderEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        var CrusaderForts = ModifierUtils.GetActiveModifiers<CrusaderFortifiedModifier>();

        if (!CrusaderForts.Any())
        {
            return;
        }

        foreach (var mod in CrusaderForts)
        {
            var genOpt = OptionGroupSingleton<GeneralOptions>.Instance;
            var show = OptionGroupSingleton<CrusaderOptions>.Instance.ShowFortified;

            var showShieldedEveryone = show == FortifyOptions.Everyone;
            var showShieldedSelf = PlayerControl.LocalPlayer.PlayerId == mod.Player.PlayerId &&
                                   show is FortifyOptions.Self or FortifyOptions.SelfAndCrusader;
            var showShieldedCrusader = PlayerControl.LocalPlayer.PlayerId == mod.Crusader.PlayerId &&
                                     show is FortifyOptions.Crusader or FortifyOptions.SelfAndCrusader;

            var body = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x =>
                x.ParentId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);
            var fakePlayer = FakePlayer.FakePlayers.FirstOrDefault(x =>
                x.PlayerId == PlayerControl.LocalPlayer.PlayerId && !TutorialManager.InstanceExists);

            mod.ShowFort = showShieldedEveryone || showShieldedSelf || showShieldedCrusader || (PlayerControl.LocalPlayer.HasDied() && genOpt.TheDeadKnow && !body && !fakePlayer?.body);
        }
    }

    [RegisterEvent(-1)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        // Logger<TownOfSushiPlugin>.Error("CrusaderEvents KillButtonClickHandler");
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick())
        {
            return;
        }
        // Adding Warlock's button to check because:
        // 2. Warlock's curse button somehow breaks when interacting with Crusader's fortify so this is the easiest way to fix it
        if (button is IKillButton || button == CustomButtonSingleton<WarlockCurseButton>.Instance)
        {
            CheckForCrusaderFortifyMurder(@event, source, target);
        }
        else
        {
            CheckForCrusaderFortify(@event, source, target);
        }
    }

    [RegisterEvent(-1)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        CheckForCrusaderFortify(@event, source, target);
        CheckForCrusaderFortifyMurder(@event, source, target);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var victim = @event.Target;

        foreach (var Crusader in CustomRoleUtils.GetActiveRolesOfType<CrusaderRole>())
        {
            if (victim == Crusader.Fortified)
            {
                Crusader.Clear();
            }
        }
    }

    private static void CheckForCrusaderFortify(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (MeetingHud.Instance || ExileController.Instance)
        {
            return;
        }

        if (!target.HasModifier<CrusaderFortifiedModifier>() || source == target ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return;
        }

        @event.Cancel();

        // The reason this exists is that otherwise, players can brute force through the crusader fort if they spam fast enough
        if (@event is MiraButtonClickEvent buttonClick)
        {
            var button = buttonClick.Button as CustomActionButton<PlayerControl>;
            if (button != null)
            {
                button.Timer += 1f;
            }
        }
        if (@event is BeforeMurderEvent && source.IsImpostor())
        {
            source.SetKillTimer(source.killTimer + 1f);
        }

        // Find the Crusader which fortified the target
        var Crusader = target.GetModifier<CrusaderFortifiedModifier>()?.Crusader.GetRole<CrusaderRole>();

        if (Crusader != null && source.AmOwner)
        {
            CrusaderRole.RpcCrusaderNotify(Crusader.Player, source, target);
        }
    }
    private static void CheckForCrusaderFortifyMurder(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (!target.HasModifier<CrusaderFortifiedModifier>() || source == target || MeetingHud.Instance || (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield)) return;
        @event.Cancel();

        // The reason this exists is that otherwise, players can brute force through the crusader fort if they spam fast enough
        if (@event is MiraButtonClickEvent buttonClick)
        {
            var button = buttonClick.Button as CustomActionButton<PlayerControl>;
            if (button != null)
            {
                button.Timer += 1f;
            }
        }
        if (@event is BeforeMurderEvent && source.IsImpostor())
        {
            source.SetKillTimer(source.killTimer + 1f);
        }

        // Find the crusader which fortified the target
        var crusader = target.GetModifier<CrusaderFortifiedModifier>()?.Crusader.GetRole<CrusaderRole>();

        if (crusader != null && source.AmOwner && !crusader.Player.HasDied())
        {
            CrusaderRole.RpcCrusaderFortifyMurder(target, source);
        }
    }
}