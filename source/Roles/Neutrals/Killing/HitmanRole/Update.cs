
using InnerNet;

namespace TownOfSushi.Roles.Neutral.Killing.HitmanRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    internal class Update
    {
        private static void Postfix(HudManager __instance)
        {
            var Hitman = AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Hitman);
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
                if (Hitman != null)
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman))
                        GetRole<Hitman>(PlayerControl.LocalPlayer).Update(__instance);
        }
    }
}