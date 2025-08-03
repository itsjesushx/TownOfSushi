using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Buttons.Neutral;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Events.Neutral;

public static class HitmanEvents
{
    [RegisterEvent]
    public static void OnMeetingEventHandler(StartMeetingEvent @event)
    {
        foreach (var Hitman in CustomRoleUtils.GetActiveRolesOfType<HitmanRole>().Select(Hitman => Hitman.Player).ToList())
        {
            if (Hitman.HasModifier<HitmanDragModifier>())
            {
                Hitman.GetModifierComponent().RemoveModifier<HitmanDragModifier>();
            }

            if (Hitman.AmOwner)
            {
                CustomButtonSingleton<HitmanDragDropButton>.Instance.SetDrag();
            }
        }
    }
}