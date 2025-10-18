using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using Reactor.Utilities;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Modifiers.Game.Impostor;

public static class TelepathEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var victim = @event.Target;

        if (PlayerControl.LocalPlayer.HasModifier<TelepathModifier>() && !source.AmOwner && !victim.AmOwner)
        {
            var options = OptionGroupSingleton<TelepathOptions>.Instance;
            if (victim.IsImpostor() && source == victim && options.KnowFailedGuess && MeetingHud.Instance &&
                victim.TryGetModifier<ImpostorAssassinModifier>(out var assassin) && assassin.LastAttemptedVictim)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.ImpSoft, alpha: 0.4f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.ImpSoft, $"<b>Your teammate, {victim.Data.PlayerName}, attempted to shoot {assassin.LastAttemptedVictim!.Data.PlayerName} as {assassin.LastGuessedItem}, but failed!</b>"),
                    Color.white, spr: TOSModifierIcons.Telepath.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (source.IsImpostor() && source != victim && options.KnowCorrectGuess && MeetingHud.Instance)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.ImpSoft, alpha: 0.05f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.ImpSoft, $"<b>Your teammate, {source.Data.PlayerName}, shot {victim.Data.PlayerName} as {victim.GetRoleWhenAlive().TeamColor.ToTextColor()}{victim.GetRoleWhenAlive().NiceName}!</b>"),
                    Color.white, spr: TOSModifierIcons.Telepath.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (source.IsImpostor() && source != victim)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.ImpSoft, alpha: 0.05f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.ImpSoft, $"<b>Your teammate, {source.Data.PlayerName}, has killed.</b>"),
                    Color.white, spr: TOSModifierIcons.Telepath.LoadAsset());
                
                notif1.AdjustNotification();
                if (options.KnowKillLocation)
                {
                    victim?.AddModifier<TelepathDeathNotifierModifier>(PlayerControl.LocalPlayer);
                }
            }
            else if (victim.IsImpostor() && options.KnowDeath)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.ImpSoft, alpha: 0.4f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.ImpSoft, $"<b>Your teammate, {victim.Data.PlayerName}, has been murdered!.</b>"),
                    Color.white, spr: TOSModifierIcons.Telepath.LoadAsset());
                
                notif1.AdjustNotification();
                if (options.KnowDeathLocation)
                {
                    victim?.AddModifier<TelepathDeathNotifierModifier>(PlayerControl.LocalPlayer);
                }
            }
        }
    }
}