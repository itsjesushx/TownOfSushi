using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Buttons.Impostor;
using TownOfSushi.Modifiers.Impostor;
using TownOfSushi.Roles.Impostor;

namespace TownOfSushi.Events.Impostor;

public static class UndertakerEvents
{
    [RegisterEvent]
    public static void OnMeetingEventHandler(StartMeetingEvent @event)
    {
        foreach (var undertaker in CustomRoleUtils.GetActiveRolesOfType<UndertakerRole>().Select(undertaker => undertaker.Player).ToList())
        {
            if (undertaker.HasModifier<UndertakerDragModifier>())
            {
                undertaker.GetModifierComponent().RemoveModifier<UndertakerDragModifier>();
            }

            if (undertaker.AmOwner)
            {
                CustomButtonSingleton<UndertakerDragDropButton>.Instance.SetDrag();
            }
        }
    }
}
