using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Impostor;

public static class UndertakerEvents
{
    [RegisterEvent]
    public static void OnMeetingEventHandler(StartMeetingEvent @event)
    {
        foreach (var undertaker in CustomRoleUtils.GetActiveRolesOfType<UndertakerRole>()
                     .Select(undertaker => undertaker.Player).ToList())
        {
            if (undertaker.HasModifier<DragModifier>())
            {
                undertaker.GetModifierComponent().RemoveModifier<DragModifier>();
            }

            if (undertaker.AmOwner)
            {
                CustomButtonSingleton<UndertakerDragDropButton>.Instance.SetDrag();
            }
        }
    }
}