using MiraAPI.Events;
using MiraAPI.Modifiers;
using TownOfSushi.Events.TOSEvents;
using TownOfSushi.Modifiers;

namespace TownOfSushi.Events;

public static class RevealEvents
{
    [RegisterEvent]
    public static void ChangeRoleHandler(ChangeRoleEvent @event)
    {
        if (!PlayerControl.LocalPlayer)
        {
            return;
        }

        var player = @event.Player;
        var mods = player.GetModifiers<RevealModifier>();
        foreach (var mod in mods)
        {
            if (mod.ChangeRoleResult is ChangeRoleResult.RemoveModifier)
            {
                mod.ModifierComponent?.RemoveModifier(mod);
            }
            else if (mod.ChangeRoleResult is ChangeRoleResult.UpdateInfo)
            {
                mod.ShownRole = @event.NewRole;
            }
        }
    }
}