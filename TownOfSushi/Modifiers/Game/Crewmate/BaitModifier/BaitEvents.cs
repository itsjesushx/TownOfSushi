using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using Reactor.Utilities;

namespace TownOfSushi.Modifiers.Game.Crewmate;
public static class BaitEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<BaitModifier>() && @event.Target != @event.Source &&
            !@event.Source.IsRole<SoulCollectorRole>() &&
            !MeetingHud.Instance)
        {
            Coroutines.Start(BaitModifier.CoReportDelay(@event.Source, @event.Target));
        }
    }
}