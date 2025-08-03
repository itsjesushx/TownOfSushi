using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Modifiers.Game.Universal;

namespace TownOfSushi.Events.Modifiers;

public static class ButtonBarryEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return; // Never run when round starts.
        ButtonBarryModifier.OnRoundStart();
    }
}
