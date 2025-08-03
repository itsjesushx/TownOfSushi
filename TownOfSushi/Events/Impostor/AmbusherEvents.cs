using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Roles;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Events.Impostor;

public static class AmbusherEvents
{
    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<AmbusherRole>().Do(x => x.Clear());
    }
    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        CustomRoleUtils.GetActiveRolesOfType<AmbusherRole>().Do(x => x.CheckDeadPursued());
    }
}