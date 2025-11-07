using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Crewmate;

public static class TrackerEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
         var opt = OptionGroupSingleton<TrackerOptions>.Instance;
        if (@event.Player.AmOwner && @event.Player.Data.Role is TrackerTOSRole &&
            opt.TaskUses && !opt.ResetOnNewRound)
        {
            var button = CustomButtonSingleton<TrackerTrackButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        if (!OptionGroupSingleton<TrackerOptions>.Instance.ResetOnNewRound)
        {
            return;
        }

        foreach (var tracker in CustomRoleUtils.GetActiveRolesOfType<TrackerTOSRole>())
        {
            tracker.Clear();
        }

        var button = CustomButtonSingleton<TrackerTrackButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<TrackerOptions>.Instance.MaxTracks);
    }
}