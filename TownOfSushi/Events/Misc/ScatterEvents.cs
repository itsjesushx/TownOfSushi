using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Events.Misc;

public static class ScatterEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        //Logger<TownOfSushiPlugin>.Error($"ScatterEvents - RoundStartHandler");

        ModifierUtils.GetActiveModifiers<ScatterModifier>().Do(x => x.OnRoundStart());
    }
}