using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;

namespace TownOfSushi.Events.Modifiers;

public static class SatelliteEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return; // Never run when round starts.
        ModifierUtils.GetActiveModifiers<SatelliteModifier>().Do(x => x.OnRoundStart());
    }
}
