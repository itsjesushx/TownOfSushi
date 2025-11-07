using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;

namespace TownOfSushi.Modifiers.Game.Impostor;

public static class UnderdogEvents
{
    [RegisterEvent(1)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        // BountyHunter already handles it's own Kill timer
        if (!source.HasModifier<UnderdogModifier>() || source.IsRole<BountyHunterRole>())
        {
            return;
        }

        source.SetKillTimer(source.GetKillCooldown());
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (!PlayerControl.LocalPlayer.HasModifier<UnderdogModifier>() ||
            PlayerControl.LocalPlayer.IsRole<BountyHunterRole>())
        {
            return;
        }

        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.GetKillCooldown());
    }
}