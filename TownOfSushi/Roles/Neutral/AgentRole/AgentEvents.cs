using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;

namespace TownOfSushi.Roles.Neutral;

public static class AgentEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is AgentRole Agent)
        {
            Agent.CheckTaskRequirements();
        }
    }
}