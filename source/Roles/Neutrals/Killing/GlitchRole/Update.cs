
using InnerNet;

namespace TownOfSushi.Roles.Neutral.Killing.GlitchRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class Update
    {
        private static void Postfix(HudManager __instance)
        {
            var glitch = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (glitch != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                        GetRole<Glitch>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }
}