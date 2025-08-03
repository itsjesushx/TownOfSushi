using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Events.Neutral;

public static class PlaguebearerEvents
{
    [RegisterEvent]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        if (!@event.Target) return;

        PlaguebearerRole.CheckInfected(@event.Target!.Object, @event.Reporter);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        PlaguebearerRole.CheckInfected(@event.Source, @event.Target);
    }

    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (@event.Button is PlaguebearerInfectButton) return;
        if (target == null || button == null || !button.CanClick()) return;

        PlaguebearerRole.RpcCheckInfected(source, target);
    }
}
