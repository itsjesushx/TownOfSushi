using HarmonyLib;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Player;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using TownOfSushi.Modifiers;
using TownOfSushi.Modifiers.Game.Alliance;
using TownOfSushi.Options.Modifiers.Alliance;
using TownOfSushi.Utilities;

namespace TownOfSushi.Events.Modifiers;

public static class LoverEvents
{
    [RegisterEvent(400)]
    public static void PlayerDeathEventHandler(PlayerDeathEvent @event)
    {
        if (!PlayerControl.LocalPlayer.IsHost())
        {
            return;
        }
        if (@event.Player == null || !@event.Player.TryGetModifier<LoverModifier>(out var loveMod)
            || !OptionGroupSingleton<LoversOptions>.Instance.BothLoversDie || loveMod.OtherLover == null
            || loveMod.OtherLover.HasDied() || loveMod.OtherLover.HasModifier<InvulnerabilityModifier>())
        {
            return;
        }
        switch (@event.DeathReason)
        {
            case DeathReason.Exile:
                loveMod.OtherLover.RpcPlayerExile();
                DeathHandlerModifier.RpcUpdateDeathHandler(loveMod.OtherLover, "Heartbreak", DeathEventHandlers.CurrentRound, DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                break;
            case DeathReason.Kill:
                loveMod.OtherLover.RpcCustomMurder(loveMod.OtherLover);
                DeathHandlerModifier.RpcUpdateDeathHandler(loveMod.OtherLover, "Heartbreak", DeathEventHandlers.CurrentRound,
                    (!MeetingHud.Instance && !ExileController.Instance) ? DeathHandlerOverride.SetTrue : DeathHandlerOverride.SetFalse, lockInfo: DeathHandlerOverride.SetTrue);
                break;
        }
    }

    [RegisterEvent]
    public static void RoundStartHandler(RoundStartEvent @event)
    {
        ModifierUtils.GetActiveModifiers<LoverModifier>().Do(x => x.OnRoundStart());
    }
}