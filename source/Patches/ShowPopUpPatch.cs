using System.Text;

//show lore
namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
    public static class ShowPopUpPatch
    {
        public static void Postfix()
        {
            var role = GetPlayerRole(PlayerControl.LocalPlayer);
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (role == null) return;

            try
            {
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    try
                    {
                        // Roles Description
                        var stringb = new StringBuilder();
                        stringb.Append(ColorString(role.Color, $"{role.Name} Description: \n"));
                        stringb.Append(ColorString(role.Color, $"{role.RoleInfo}"));
                        HudManager.Instance.ShowPopUp(stringb.ToString());
                    }
                    catch (Exception exs)
                    {
                        TownOfSushi.Logger.LogError("Error:  " + exs);
                        throw;
                    }
                }

                if (Input.GetKeyDown(KeyCode.F4))
                {
                    try
                    {
                        // Roles lore
                        var stringb = new StringBuilder();
                        stringb.Append(ColorString(role.Color, $"{role.Name} lore: \n"));
                        stringb.Append(ColorString(role.Color, $"{role.LoreText}"));
                        HudManager.Instance.ShowPopUp(stringb.ToString());
                    }
                    catch (Exception exs)
                    {
                        TownOfSushi.Logger.LogError("Error:  " + exs);
                        throw;
                    }
                }
            }           
            catch (Exception errors)
            {
                TownOfSushi.Logger.LogError("Error:  " + errors);
            }
        }    
    }
}