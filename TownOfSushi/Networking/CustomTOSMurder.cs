using MiraAPI.Networking;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;

namespace TownOfSushi.Networking;

public static class CustomTouMurderRpcs
{
    /// <summary>
    /// Networked Custom Murder method.
    /// </summary>
    /// <param name="source">The killer.</param>
    /// <param name="target">The player to murder.</param>
    [MethodRpc((uint)TownOfSushiRpc.GhostRoleMurder, LocalHandling = RpcLocalHandling.Before, SendImmediately = true)]
    public static void RpcGhostRoleMurder(
        this PlayerControl source,
        PlayerControl target)
    {
        if (!source.HasDied() || target.HasDied())
        {
            return;
        }
        var cod = "killed";
        var role = source.GetRoleWhenAlive();
        if (source.Data.Role is IGhostRole)
        {
            role = source.Data.Role;
        }
        var touRole = role as ITownOfSushiRole;
        if (touRole == null || touRole.RoleAlignment is not RoleAlignment.NeutralEvil)
        {
            return;
        }
        source.AddModifier<IndirectAttackerModifier>(true);
        
        switch (role)
        {
            case PhantomTOSRole:
            cod = "Spooked";
            break;
            case JesterRole:
            cod = "Haunted";
            break;
            case ExecutionerRole:
            cod = "Tormented";
            break;
        }
        
        DeathHandlerModifier.UpdateDeathHandler(target, cod, DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetTrue, $"By {source.Data.PlayerName}", lockInfo: DeathHandlerOverride.SetTrue);
        DeathHandlerModifier.UpdateDeathHandler(source, "null", -1, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
        source.CustomMurder(
            target,
            MurderResultFlags.Succeeded);
        
        source.RemoveModifier<IndirectAttackerModifier>();
    }
}