using MiraAPI.Events;
using MiraAPI.Events.Mira;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Hud;
using TownOfSushi.Buttons;

namespace TownOfSushi.Modifiers.Game.Universal
{
    public static class ArmoredEvents
    {
        [RegisterEvent]
        public static void MiraButtonCancelledEventHandler(MiraButtonCancelledEvent @event)
        {
            var source = PlayerControl.LocalPlayer;
            var button = @event.Button as CustomActionButton<PlayerControl>;
            var target = button?.Target;
            if (target == null || button is not IKillButton)
            {
                return;
            }

            if (target && !target!.HasModifier<ArmoredModifier>())
            {
                return;
            }

            ResetButtonTimer(source, button);
        }

        [RegisterEvent(1)]
        public static void BeforeMurderEventHandler(BeforeMurderEvent @event)
        {
            var source = @event.Source;
            var target = @event.Target;

            CheckForActive(@event, source, target);
        }

        private static void ResetButtonTimer(PlayerControl source, CustomActionButton<PlayerControl>? button = null)
        {
            var reset = OptionGroupSingleton<ArmoredOptions>.Instance.ResetCooldown;
            button?.SetTimer(reset);

            // Reset impostor kill cooldown if they attack a shielded player
            if (!source.AmOwner || !source.IsImpostor())
            {
                return;
            }

            source.SetKillTimer(reset);
        }

        private static void CheckForActive(MiraCancelableEvent miraEvent, PlayerControl source, PlayerControl target)
        {
            if (MeetingHud.Instance || ExileController.Instance)
            {
                return;
            }

            source.TryGetModifier<IndirectAttackerModifier>(out var indirectMod); // yeah i do not know what this is but /shrug
            if (target.HasModifier<ArmoredModifier>() && source != target && !IsAlreadyProtected(target))
            {
                var armored = target.GetModifier<ArmoredModifier>();
                if (armored != null && armored.isActive)
                {
                    armored.isActive = false;
                    if (indirectMod == null || !indirectMod.IgnoreShield)
                    {
                        miraEvent.Cancel();
                        ResetButtonTimer(source);
                        if (PlayerControl.LocalPlayer == source)
                        {
                            target.ShowFailedMurder();
                        }
                    }
                }
            }
        }

        public static bool IsAlreadyProtected(PlayerControl player)
        {
            return
                player.HasModifier<GuardianAngelProtectModifier>() ||
                player.HasModifier<AmnesiacVestModifier>() ||
                (player.HasModifier<VeteranAlertModifier>() && OptionGroupSingleton<VeteranOptions>.Instance.KilledOnAlert) ||
                player.HasModifier<ClericBarrierModifier>() ||
                player.HasModifier<MedicShieldModifier>() ||
                player.HasModifier<RomanticProtectModifier>() ||
                player.HasModifier<FirstDeadShield>() ||
                (player.Data.Role is PestilenceRole);
        }
    }
}