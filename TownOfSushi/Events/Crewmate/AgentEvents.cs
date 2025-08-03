using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Events.Crewmate;

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