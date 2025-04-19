namespace TownOfSushi
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.HandleHud))]
    public class KeyboardJoystickPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            if (HUDManager() != null && HUDManager().ImpostorVentButton != null && HUDManager().ImpostorVentButton.isActiveAndEnabled && ConsoleJoystick.player.GetButtonDown(50))
                HUDManager().ImpostorVentButton.DoClick();
        }
    }
}
