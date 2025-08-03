using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using Reactor.Utilities;
using TownOfSushi.Modifiers.Game.Crewmate;
using TownOfSushi.Roles.Neutral;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Modifiers;

public static class DecayEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (@event.Target.HasModifier<DecayModifier>() && !@event.Source.IsRole<SoulCollectorRole>() && !MeetingHud.Instance)
        {
            Coroutines.Start(DecayModifier.StartDecay(@event.Target));
        }
    }
}
