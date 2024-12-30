namespace TownOfSushi.Roles.Neutral.Evil.JesterRole
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.BeginForGameplay))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer;
            if (exiled == null) return;
            var player = exiled.Object;

            var role = GetPlayerRole(player);
            if (role == null) return;
            if (role.RoleType == RoleEnum.Jester)
            {
                ((Jester)role).Wins();
            }
        }
    }
}