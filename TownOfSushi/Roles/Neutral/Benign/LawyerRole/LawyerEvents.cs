using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Roles;
public static class LawyerEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        foreach (var lawyer in CustomRoleUtils.GetActiveRolesOfType<LawyerRole>())
        {
            lawyer.CheckTargetDeath(@event.Target);
        }
    }

    // Exile the Lawyer as punishment for not protecting their client!
    [RegisterEvent(0)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason is DeathReason.Exile)
        {
            if (!@event.Player.TryGetModifier<LawyerClientModifier>(out var clientMod))
            {
                return;
            }

            var lawyer = GameData.Instance.GetPlayerById(clientMod.OwnerId).Object;
            if (lawyer != null && !lawyer.HasDied() && lawyer.Data.Role is LawyerRole)
            {
                lawyer.Exiled();
                DeathHandlerModifier.RpcUpdateDeathHandler(lawyer, "Client Voted Out", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
            }
        }
     /*   else
        {
            CustomRoleUtils.GetActiveRolesOfType<LawyerRole>().Do(x => x.CheckTargetDeath(@event.Player));
        }*/
    }
}