using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Roles.Impostor;

public static class BountyHunterEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        if (!source.AmOwner || !source.IsRole<BountyHunterRole>())
        {
            return;
        }

        var bountyHunter = source.GetRole<BountyHunterRole>();
        bountyHunter?.OnPlayerKilled(@event.Target);
    }
}