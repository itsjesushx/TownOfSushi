namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class CamouflageUnCamouflagePatch
    {
        public static bool CommsEnabled;
        public static bool IsCamouflaged => CommsEnabled;
        public static void Postfix(HudManager __instance)
        {
            if (CustomGameOptions.ColourblindComms)
            {
                if (ShipStatus.Instance != null)
                    switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                    {
                        default:
                        case 0:
                        case 2:
                        case 3:
                        case 4:
                        case 6:
                            var comms1 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HudOverrideSystemType>();
                            if (comms1.IsActive)
                            {
                                CommsEnabled = true;
                                GroupCamouflage();
                                return;
                            }

                            break;
                        case 1:
                        case 5:
                            var comms2 = ShipStatus.Instance.Systems[SystemTypes.Comms].Cast<HqHudSystemType>();
                            if (comms2.IsActive)
                            {
                                CommsEnabled = true;
                                GroupCamouflage();
                                return;
                            }

                            break;
                    }

                if (CommsEnabled)
                {
                    CommsEnabled = false;
                    UnCamouflage();
                }
            }
        }
    }
}