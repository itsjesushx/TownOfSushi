using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Modules;

namespace TownOfSushi.Roles.Neutral;

public static class SoulCollectorEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (source.IsRole<SoulCollectorRole>() && !MeetingHud.Instance)
        {
            _ = new FakePlayer(target);
        }
    }
}