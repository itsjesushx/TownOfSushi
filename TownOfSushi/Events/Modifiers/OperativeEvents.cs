using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Game.Crewmate;

namespace TownOfSushi.Events.Crewmate;

public static class OperativeEvents
{

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.HasModifier<OperativeModifier>() && @event.Player.AmOwner)
        {
            OperativeModifier.OnTaskComplete();
        }
    }
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro) return; // Never run when round starts.
        if (PlayerControl.LocalPlayer.HasModifier<OperativeModifier>()) OperativeModifier.OnRoundStart();
    }
}
