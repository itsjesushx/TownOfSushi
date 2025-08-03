using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Modules;
using TownOfSushi.Options.Roles.Crewmate;
using TownOfSushi.Roles;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Neutral;

public static class SheriffEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            CustomButtonSingleton<SheriffShootButton>.Instance.FailedShot = false;
        }
        else if (PlayerControl.LocalPlayer.Data.Role is SheriffRole)
        {
            SheriffRole.OnRoundStart();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (source.Data.Role is not SheriffRole)
        {
            return;
        }

        if (source.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished)
        {
            return;
        }

        var target = @event.Target;
        var options = OptionGroupSingleton<SheriffOptions>.Instance;

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (target.IsImpostor() ||
                (target.IsCrewmate() && target.TryGetModifier<AllianceGameModifier>(out var allyMod) &&
                 !allyMod.GetsPunished) ||
                (target.Is(RoleAlignment.NeutralEvil) && options.ShootNeutralEvil) ||
                (target.Is(RoleAlignment.NeutralKilling) && options.ShootNeutralKiller))
            {
                stats.CorrectKills += 1;
            }
            else
            {
                stats.IncorrectKills += 1;
            }
        }
    }
}