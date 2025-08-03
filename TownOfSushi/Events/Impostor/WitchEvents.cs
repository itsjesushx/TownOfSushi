using MiraAPI.Events;
using MiraAPI.Roles;
using MiraAPI.Modifiers;
using MiraAPI.Events.Vanilla.Meeting;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Neutral;

public static class WitchEvents
{
    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<WitchRole>().Any()) return;

        foreach (var player in PlayerControl.AllPlayerControls)
        {
            foreach (var spell in player.GetModifiers<WitchSpelledModifier>())
            {
                var witchPlayer = MiscUtils.PlayerById(spell.WitchId);
                if (witchPlayer != null)
                {
                    WitchRole.RpcMurderCursedPlayer(witchPlayer, player);
                    break;
                }
            }
        }
    }
}