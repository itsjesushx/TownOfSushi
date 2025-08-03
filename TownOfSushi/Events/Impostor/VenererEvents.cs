using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons.Impostor;
using TownOfSushi.Roles.Impostor;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Impostor;

public static class VenererEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!@event.Source.AmOwner || !@event.Source.IsRole<VenererRole>() || MeetingHud.Instance)
        {
            return;
        }

        var button = CustomButtonSingleton<VenererAbilityButton>.Instance;
        if (button.ActiveAbility != VenererAbility.Freeze)
        {
            button.UpdateAbility(button.ActiveAbility + 1);
        }
    }
}