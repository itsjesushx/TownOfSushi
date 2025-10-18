using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using Reactor.Utilities;
using TownOfSushi.Events;
using TownOfSushi.Modifiers;
using TownOfSushi.Modules;
using UnityEngine;

namespace TownOfSushi.Roles.Neutral;

public static class InquisitorEvents
{
    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var source = @event.Source;
        var victim = @event.Target;

        if (source.Data.Role is InquisitorRole inquis &&
            GameHistory.PlayerStats.TryGetValue(source.PlayerId, out var stats))
        {
            if (victim.HasModifier<InquisitorHereticModifier>())
            {
                stats.CorrectKills += 1;
            }
            else if (source != victim)
            {
                stats.IncorrectKills += 1;
                inquis.CanVanquish = false;
            }
        }

        if (PlayerControl.LocalPlayer.Data.Role is InquisitorRole)
        {
            if (victim.HasModifier<InquisitorHereticModifier>() && !victim.AmOwner && !source.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Inquisitor, alpha: 0.1f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>A Heretic has perished!</b>"), Color.white,
                    spr: TOSRoleIcons.Inquisitor.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (!victim.HasModifier<InquisitorHereticModifier>() && !victim.AmOwner && source.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Inquisitor, alpha: 0.4f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>{victim.Data.PlayerName} was not a heretic!\nYou can no longer vanquish players.</b>"),
                    Color.white, spr: TOSRoleIcons.Inquisitor.LoadAsset());
                
                notif1.AdjustNotification();
            }
            else if (victim.HasModifier<InquisitorHereticModifier>() && !victim.AmOwner && source.AmOwner)
            {
                Coroutines.Start(MiscUtils.CoFlash(TownOfSushiColors.Doomsayer, alpha: 0.4f));
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>{victim.Data.PlayerName} was a heretic!</b>"),
                    Color.white, spr: TOSRoleIcons.Inquisitor.LoadAsset());
                
                notif1.AdjustNotification();
            }
        }

        CustomRoleUtils.GetActiveRolesOfType<InquisitorRole>().Do(x => x.CheckTargetDeath(victim));
    }

    [RegisterEvent]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (@event.DeathReason != DeathReason.Exile)
        {
            return;
        }

        CustomRoleUtils.GetActiveRolesOfType<InquisitorRole>().Do(x => x.CheckTargetDeath(@event.Player));
    }

    [RegisterEvent]
    public static void RoundStartEventHandler(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro)
        {
            return;
        }

        var inquis = CustomRoleUtils.GetActiveRolesOfType<InquisitorRole>().FirstOrDefault();
        if (inquis != null && inquis.TargetsDead && !inquis.Player.HasDied())
        {
            if (inquis.Player.AmOwner)
            {
                PlayerControl.LocalPlayer.RpcPlayerExile();
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>You have successfully won as the Inquisitor, as all Heretics have perished!</b>"),
                    Color.white, spr: TOSRoleIcons.Inquisitor.LoadAsset());

                
                notif1.AdjustNotification();
                DeathHandlerModifier.RpcUpdateDeathHandler(PlayerControl.LocalPlayer, "Victorious", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
            }
            else
            {
                var notif1 = Helpers.CreateAndShowNotification(
                    MiscUtils.ColorString(TownOfSushiColors.Inquisitor, $"<b>The Inquisitor, {inquis.Player.Data.PlayerName}, has successfully won, as all Heretics have perished!</b>"),
                    Color.white, spr: TOSRoleIcons.Inquisitor.LoadAsset());

                
                notif1.AdjustNotification();
            }
        }
    }
}