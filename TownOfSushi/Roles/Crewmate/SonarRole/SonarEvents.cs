using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Crewmate;

public static class SonarEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
         var opt = OptionGroupSingleton<SonarOptions>.Instance;
        if (@event.Player.AmOwner && @event.Player.Data.Role is SonarRole &&
            opt.TaskUses && !opt.ResetOnNewRound)
        {
            var button = CustomButtonSingleton<SonarTrackButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }

    [RegisterEvent]
    public static void EjectionEventEventHandler(EjectionEvent @event)
    {
        if (!OptionGroupSingleton<SonarOptions>.Instance.ResetOnNewRound)
        {
            return;
        }

        foreach (var tracker in CustomRoleUtils.GetActiveRolesOfType<SonarRole>())
        {
            tracker.Clear();
        }

        var button = CustomButtonSingleton<SonarTrackButton>.Instance;
        button.SetUses((int)OptionGroupSingleton<SonarOptions>.Instance.MaxTracks);
    }
}