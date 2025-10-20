using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Roles.Neutral;

public static class ThiefEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (source.IsRole<ThiefRole>())
        {
            ThiefRole.RpcStealRole(source, target);
        }
    }
}