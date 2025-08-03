﻿using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using TownOfSushi.Buttons;
using TownOfSushi.Buttons.Crewmate;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Crewmate;
using TownOfSushi.Options;
using TownOfSushi.Roles.Crewmate;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Crewmate;

public static class MedicEvents
{
    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        if (PlayerControl.LocalPlayer.Data.Role is MedicRole)
        {
            MedicRole.OnRoundStart();
        }
    }
    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (CheckForMedicShield(@event, source, target))
        {
            ResetButtonTimer(source);
        }
    }

    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;
        if (target == null || button is not IKillButton) return;

        if (CheckForMedicShield(@event, source, target))
        {
            ResetButtonTimer(source, button);
        }
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        var victim = @event.Target;

        foreach (var medic in CustomRoleUtils.GetActiveRolesOfType<MedicRole>())
        {
            if (victim == medic.Shielded)
                medic.Clear();
        }
        if (victim.TryGetModifier<MedicShieldModifier>(out var medMod)
            && PlayerControl.LocalPlayer.Data.Role is MedicRole
            && medMod.Medic.AmOwner)
        {
            CustomButtonSingleton<MedicShieldButton>.Instance.CanChangeTarget = true;
        }
    }

    [RegisterEvent]
    public static void EjectionEventHandler(EjectionEvent @event)
    {
        var exiled = @event.ExileController?.initData?.networkedPlayer?.Object;
        if (exiled == null)
        {
            return;
        }

        if (exiled.TryGetModifier<MedicShieldModifier>(out var medMod)
            && PlayerControl.LocalPlayer.Data.Role is MedicRole
            && medMod.Medic.AmOwner)
        {
            CustomButtonSingleton<MedicShieldButton>.Instance.CanChangeTarget = true;
        }
    }

    [RegisterEvent]
    public static void PlayerLeaveEventHandler(PlayerLeaveEvent @event)
    {
        var player = @event.ClientData.Character;

        if (!player)
        {
            return;
        }

        if (player && player.TryGetModifier<MedicShieldModifier>(out var medMod)
            && PlayerControl.LocalPlayer.Data.Role is MedicRole
            && medMod.Medic.AmOwner)
        {
            CustomButtonSingleton<MedicShieldButton>.Instance.CanChangeTarget = true;
        }
    }

    [RegisterEvent]
    public static void ReportBodyEventHandler(ReportBodyEvent @event)
    {
        if (@event.Target == null)
        {
            return;
        }

        if (@event.Reporter.Data.Role is MedicRole medic && @event.Reporter.AmOwner)
        {
            medic.Report(@event.Target.PlayerId);
        }
    }

    private static bool CheckForMedicShield(MiraCancelableEvent @event, PlayerControl source, PlayerControl target)
    {
        if (!target.HasModifier<MedicShieldModifier>() || 
            MeetingHud.Instance ||
            source == null ||
            target.PlayerId == source.PlayerId || 
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return false;
        }

        @event.Cancel();

        var medic = target.GetModifier<MedicShieldModifier>()?.Medic.GetRole<MedicRole>();

        if (medic != null && source.AmOwner)
        {
            MedicRole.RpcMedicShieldAttacked(medic.Player, source, target);
        }

        return true;
    }

    private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null)
    {
        var reset = OptionGroupSingleton<GeneralOptions>.Instance.TempSaveCdReset;

        button?.SetTimer(reset);

        // Reset impostor kill cooldown if they attack a shielded player
        if (!source.AmOwner || !source.IsImpostor()) return;

        source.SetKillTimer(reset);
    }
}
