using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;

namespace TownOfSushi.Roles.Impostor;

public static class EscapistEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<EscapistRole>().Do(x => x.MarkedLocation = null);
    }
}