using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;

namespace TownOfSushi.Roles.Crewmate;

public static class InformantEvents
{
    [RegisterEvent]
    public static void DeathEvent(PlayerDeathEvent @event)
    {
        if (InformantRole.IsTargetOfInformant(@event.Player))
        {
            CustomRoleUtils.GetActiveRolesOfType<InformantRole>().ToList()
                .ForEach(snitch => snitch.RemoveArrowForPlayer(@event.Player.PlayerId));
        }
    }

    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        if (@event.Player.Data.Role is InformantRole snitch)
        {
            snitch.CanSeeRealKiller = true;
            snitch.CheckTaskRequirements();
        }
    }
}