namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    class EmergencyMinigameUpdatePatch 
    {
        static void Postfix(EmergencyMinigame __instance) 
        {
            var CanCallEmergency = true;
            var StatusText = "";

            // Potentially deactivate emergency button for Jester
            if (LocalPlayer().Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Jester can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Executioner
            if (LocalPlayer().Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Executioner can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Swapper
            if (LocalPlayer().Is(RoleEnum.Swapper) && !CustomGameOptions.SwapperButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Swapper can't start an emergency meeting";
            }

            // Potentially deactivate emergency button when 2 players are left
            if (TwoPlayersAlive())
            {
                CanCallEmergency = false;
                StatusText = "2 Players alive only. Impossible to start a meeting!";
            }

            if (!CanCallEmergency) 
            {
                __instance.StatusText.text = StatusText;
                __instance.NumberText.text = string.Empty;
                __instance.ClosedLid.gameObject.SetActive(true);
                __instance.OpenLid.gameObject.SetActive(false);
                __instance.ButtonActive = false;
                return;
            }
        }
    }
}