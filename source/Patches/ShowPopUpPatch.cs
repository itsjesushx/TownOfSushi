using System.Text;

//show lore
namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(ControllerManager), nameof(ControllerManager.Update))]
    public static class ShowLore
    {
        public static void Postfix()
        {
            var role = GetPlayerRole(PlayerControl.LocalPlayer);
            if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started) return;
            if (CustomGameOptions.GameMode == GameMode.KillersOnly) return;
            if (role == null) return;

            try
            {
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    try
                    {
                        // Roles Description
                        var stringb = new StringBuilder();
                        stringb.Append(ColorString(role.Color, $"<size=60%>{role.Name} Description:</size> \n"));
                        stringb.Append(ColorString(role.Color, $"<size=60%>{role.RoleInfo}</size>"));
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
                        stringb.Append(ColorString(role.Color, $"<size=60%>{role.Name} lore:</size> \n"));
                        stringb.Append(ColorString(role.Color, $"<size=60%>{role.LoreText}</size>"));
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