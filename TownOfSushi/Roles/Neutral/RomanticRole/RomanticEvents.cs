using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;
using TownOfSushi.Modifiers;
using TownOfSushi.Options;

namespace TownOfSushi.Roles.Neutral;

public static class RomanticEvents
{
    [RegisterEvent]
    public static void MiraButtonClickEventHandler(MiraButtonClickEvent @event)
    {
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;

        if (target == null || button == null || !button.CanClick() || button is not IKillButton) return;

        CheckRomanticProtection(@event, target);
    }

    [RegisterEvent]
    public static void MiraButtonCancelledEventHandler(MiraButtonCancelledEvent @event)
    {
        var source = PlayerControl.LocalPlayer;
        var button = @event.Button as CustomActionButton<PlayerControl>;
        var target = button?.Target;
        if (target == null || button is not IKillButton) return;

        if (target && !target!.HasModifier<RomanticProtectModifier>()) return;

        ResetButtonTimer(source, button);
    }

    [RegisterEvent]
    public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
    {
        var source = @event.Source;
        var target = @event.Target;

        if (!CheckRomanticProtection(@event, target, source)) return;
        ResetButtonTimer(source);
    }

    [RegisterEvent]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        foreach (var rom in CustomRoleUtils.GetActiveRolesOfType<RomanticRole>())
        {
            rom.CheckTargetDeath(@event.Target);
        }
    }

    private static bool CheckRomanticProtection(MiraCancelableEvent @event, PlayerControl target, PlayerControl? source=null)
    {
        if (!target.HasModifier<RomanticProtectModifier>() ||
            MeetingHud.Instance ||
            source == null ||
            source.PlayerId == target.PlayerId ||
            (source.TryGetModifier<IndirectAttackerModifier>(out var indirect) && indirect.IgnoreShield))
        {
            return false;
        }

        @event.Cancel();

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