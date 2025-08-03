
using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfSushi.Modifiers.Neutral;
using TownOfSushi.Options.Roles.Neutral;
using TownOfSushi.Roles.Neutral;

namespace TownOfSushi.Events.Neutral;

public static class ArsonistEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var source = PlayerControl.LocalPlayer;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick() || target.Data.Role is not ArsonistRole || !OptionGroupSingleton<ArsonistOptions>.Instance.DouseInteractions) return;

        if (!source.HasModifier<ArsonistDousedModifier>()) source.RpcAddModifier<ArsonistDousedModifier>(target.PlayerId);
    }
}