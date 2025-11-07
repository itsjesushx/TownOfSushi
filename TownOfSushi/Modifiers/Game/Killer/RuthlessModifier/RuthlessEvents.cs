/*using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Modfiers.Game.Killer;

public static class RuthlessEvents
{
    [RegisterEvent(-2)]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (target.IsProtected())
        {
            ResetButtonTimer(source);
        }
    }
}*/