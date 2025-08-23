using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Modifiers.Game;
using TownOfSushi.Modules;

namespace TownOfSushi.Roles.Neutral;

public static class VigilanteEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            CustomButtonSingleton<VigilanteShootButton>.Instance.FailedShot = false;
        }
        else if (PlayerControl.LocalPlayer.Data.Role is VigilanteRole)
        {
            VigilanteRole.OnRoundStart();
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;

        if (source.Data.Role is not VigilanteRole)
        {
            return;
        }

        if (source.TryGetModifier<AllianceGameModifier>(out var allyMod2) && !allyMod2.GetsPunished)
        {
            return;
        }

        var target = @event.Target;
        var options = OptionGroupSingleton<VigilanteOptions>.Instance;

        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (target.IsKillerRole() ||
                (target.IsCrewmate() && target.TryGetModifier<AllianceGameModifier>(out var allyMod) &&
                 !allyMod.GetsPunished) ||
                (target.Is(RoleAlignment.NeutralEvil) && options.ShootNeutralEvil))
            {
                stats.CorrectKills += 1;
            }
            else
            {
                stats.IncorrectKills += 1;
            }
        }
        if (GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats2) && MeetingHud.Instance)
        {
            if (source != target)
            {
                stats2.CorrectAssassinKills++;
            }
            else
            {
                stats2.CorrectAssassinKills--;
            }
        }
    }
}