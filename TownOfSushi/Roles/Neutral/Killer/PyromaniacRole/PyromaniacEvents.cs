using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Neutral;

public static class PyromaniacEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick() || target.Data.Role is not PyromaniacRole ||
            !OptionGroupSingleton<PyromaniacOptions>.Instance.DouseInteractions)
        {
            return;
        }

        if (!source.HasModifier<PyromaniacDousedModifier>())
        {
            source.RpcAddModifier<PyromaniacDousedModifier>(target.PlayerId);
        }
    }
}