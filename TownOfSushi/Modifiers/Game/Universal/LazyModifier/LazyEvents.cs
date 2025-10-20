using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Modifiers.Game.Universal;

public static class LazyEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        ModifierUtils.GetActiveModifiers<LazyModifier>().Do(x => x.OnRoundStart());
    }
}