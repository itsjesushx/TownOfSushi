using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;

namespace TownOfSushi.Events.Modifiers;

public static class ImmovableEvents
{
    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return;

        ModifierUtils.GetActiveModifiers<ImmovableModifier>().Do(x => x.OnRoundStart());
    }
}
