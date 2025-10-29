using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;

namespace TownOfSushi.Modifiers.Game.Crewmate;

public static class AdministratorEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.HasModifier<AdministratorModifier>() && @event.Player.AmOwner)
        {
            AdministratorModifier.OnTaskComplete();
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Never run when round starts.
        }

        if (PlayerControl.LocalPlayer.HasModifier<AdministratorModifier>())
        {
            AdministratorModifier.OnRoundStart();
        }
    }
}