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
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Jester can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Executioner
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Executioner can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Swapper
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !CustomGameOptions.SwapperButton) 
            {
                CanCallEmergency = false;
                StatusText = "The Swapper can't start an emergency meeting";
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
    [HarmonyPatch(typeof(VitalsMinigame), nameof(VitalsMinigame.Begin))]
    public class NoVitals
    {
        public static bool Prefix(VitalsMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter) && !CustomGameOptions.TransporterVitals)
            {
                Object.Destroy(__instance.gameObject);
                return false;
            }
            return true;
        }
    }
}