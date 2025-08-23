using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Usables;

namespace TownOfSushi.Roles.Neutral;

public static class PredatorEvents
{
    [RegisterEvent]
    public static void PlayerCanUseEventHandler(PlayerCanUseEvent @event)
    {
        if (!@event.IsVent)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer)
        {
            return;
        }

        var vent = @event.Usable.TryCast<Vent>();

        if (vent == null)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer.inVent && PlayerControl.LocalPlayer.Data.Role is PredatorRole werewolf &&
            !werewolf.Terminating)
        {
            @event.Cancel();
        }
    }
}