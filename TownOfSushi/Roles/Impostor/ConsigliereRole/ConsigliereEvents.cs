/*using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Impostor;

public static class ConsigliereEvents
{
    [RegisterEvent(-1)]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick() || !source.IsRole<ConsigliereRole>())
        {
            return;
        }

        source.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
    }
}*/