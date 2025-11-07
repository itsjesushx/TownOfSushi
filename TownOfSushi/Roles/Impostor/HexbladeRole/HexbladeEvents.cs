using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;

namespace TownOfSushi.Roles.Impostor;

public static class HexbladeEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!@event.Source.AmOwner || !@event.Source.IsRole<HexbladeRole>())
        {
            return;
        }

        var button = CustomButtonSingleton<HexbladeKillButton>.Instance;
        if (button.BurstActive)
        {
            ++button.Kills;
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return; // Only run when round starts.
        }

        var button = CustomButtonSingleton<HexbladeKillButton>.Instance;
        button.Charge = 0f;
        button.BurstActive = false;
        button.ResetCooldownAndOrEffect();
    }
}