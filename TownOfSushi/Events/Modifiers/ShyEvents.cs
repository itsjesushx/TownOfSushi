using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Universal;

namespace TownOfSushi.Events.Modifiers;

public static class ShyEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        ModifierUtils.GetActiveModifiers<ShyModifier>().Do(x => x.OnRoundStart());
    }
}
