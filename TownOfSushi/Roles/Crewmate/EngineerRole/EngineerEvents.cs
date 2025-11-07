using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Crewmate;

public static class EngineerEvents
{
    [RegisterEvent]
    public static void CompleteTaskEvent(CompleteTaskEvent @event)
    {
        var opt = OptionGroupSingleton<EngineerOptions>.Instance;
        if (@event.Player.AmOwner && @event.Player.Data.Role is EngineerTOSRole &&
            opt.TaskUses && (int)opt.MaxVents != 0)
        {
            var button = CustomButtonSingleton<EngineerVentButton>.Instance;
            ++button.UsesLeft;
            ++button.ExtraUses;
            button.SetUses(button.UsesLeft);
        }
    }
}