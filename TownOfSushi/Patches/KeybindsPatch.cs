using HarmonyLib;
using MiraAPI.Hud;
using Rewired;
using TownOfSushi.Buttons;

namespace TownOfSushi.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
public static class Bindings
{
    public static void Postfix(HudManager __instance)
    {
        if (PlayerControl.LocalPlayer == null)
        {
            return;
        }

        if (PlayerControl.LocalPlayer.Data == null)
        {
            return;
        }

        if (!PlayerControl.LocalPlayer.Data.IsDead && !PlayerControl.LocalPlayer.IsImpostor())
        {
            var kill = __instance.KillButton;
            var vent = __instance.ImpostorVentButton;

            if (kill.isActiveAndEnabled)
            {
                var killKey = ReInput.players.GetPlayer(0).GetButtonDown("ActionSecondary");
                var controllerKill = ConsoleJoystick.player.GetButtonDown(8);
                if (killKey || controllerKill)
                {
                    kill.DoClick();
                }
            }

            if (vent.isActiveAndEnabled)
            {
                var ventKey = ReInput.players.GetPlayer(0).GetButtonDown("UseVent");
                var controllerVent = ConsoleJoystick.player.GetButtonDown(50);
                if (ventKey || controllerVent)
                {
                    vent.DoClick();
                }
            }
        }

        if (ActiveInputManager.currentControlType != ActiveInputManager.InputType.Joystick)
        {
            return;
        }

        var contPlayer = ConsoleJoystick.player;
        var buttonList = CustomButtonManager.Buttons.Where(x =>
            x.Enabled(PlayerControl.LocalPlayer.Data.Role) && x.Button != null && x.Button.isActiveAndEnabled &&
            x.CanUse()).ToList();

        foreach (var button in buttonList.Where(x => x is TownOfSushiButton))
        {
            var tosButton = button as TownOfSushiButton;
            if (tosButton == null || tosButton.ConsoleBind() == -1)
            {
                continue;
            }

            if (contPlayer.GetButtonDown(tosButton.ConsoleBind()))
            {
                tosButton.PassiveComp.OnClick.Invoke();
            }
        }

        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<DeadBody>))
        {
            var tosButton = button as TownOfSushiTargetButton<DeadBody>;
            if (tosButton == null || tosButton.ConsoleBind() == -1)
            {
                continue;
            }

            if (contPlayer.GetButtonDown(tosButton.ConsoleBind()))
            {
                tosButton.PassiveComp.OnClick.Invoke();
            }
        }

        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<Vent>))
        {
            var tosButton = button as TownOfSushiTargetButton<Vent>;
            if (tosButton == null || tosButton.ConsoleBind() == -1)
            {
                continue;
            }

            if (contPlayer.GetButtonDown(tosButton.ConsoleBind()))
            {
                tosButton.PassiveComp.OnClick.Invoke();
            }
        }

        foreach (var button in buttonList.Where(x => x is TownOfSushiTargetButton<PlayerControl>))
        {
            var tosButton = button as TownOfSushiTargetButton<PlayerControl>;
            if (tosButton == null || tosButton.ConsoleBind() == -1)
            {
                continue;
            }

            if (contPlayer.GetButtonDown(tosButton.ConsoleBind()))
            {
                tosButton.PassiveComp.OnClick.Invoke();
            }
        }
    }
}