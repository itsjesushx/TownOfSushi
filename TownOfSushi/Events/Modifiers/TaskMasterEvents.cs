using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Crewmate;

namespace TownOfSushi.Events.Modifiers;

public static class TaskmasterEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        ModifierUtils.GetActiveModifiers<TaskmasterModifier>().Do(x => x.OnRoundStart());
    }
}