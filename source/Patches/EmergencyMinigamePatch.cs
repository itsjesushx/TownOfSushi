namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.Update))]
    class EmergencyMinigameUpdatePatch 
    {
        static void Postfix(EmergencyMinigame __instance) {
            var roleCanCallEmergency = true;
            var statusText = "";

            // Potentially deactivate emergency button for Jester
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jester) && !CustomGameOptions.JesterButton) {
                roleCanCallEmergency = false;
                statusText = "The Jester can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Executioner
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner) && !CustomGameOptions.ExecutionerButton) {
                roleCanCallEmergency = false;
                statusText = "The Executioner can't start an emergency meeting";
            }

            // Potentially deactivate emergency button for Snitch
            var snitch = GetRole<Snitch>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Snitch) && !CustomGameOptions.SnitchButton && snitch.Revealed) 
            {
                roleCanCallEmergency = false;
                statusText = "The Snitch can't start an emergency meeting after being revealed!";
            }

            if (!roleCanCallEmergency) 
            {
                __instance.StatusText.text = statusText;
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