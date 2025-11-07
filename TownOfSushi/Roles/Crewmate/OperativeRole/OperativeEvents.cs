using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;

namespace TownOfSushi.Roles.Crewmate;

public static class OperativeEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is OperativeRole && @event.Player.AmOwner)
        {
            OperativeRole.OnTaskComplete();
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Never run when round starts.
        }

        if (PlayerControl.LocalPlayer.Data.Role is OperativeRole)
        {
            OperativeRole.OnRoundStart();
        }
    }
}