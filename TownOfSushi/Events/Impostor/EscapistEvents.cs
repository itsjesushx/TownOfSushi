using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Roles;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Events.Impostor;

public static class EscapistEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<EscapistRole>().Do(x => x.MarkedLocation = null);
    }
}