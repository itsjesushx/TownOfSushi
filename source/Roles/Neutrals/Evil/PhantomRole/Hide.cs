namespace TownOfSushi.Roles.Neutral.Evil.PhantomRole
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Phantom))
            {
                var phantom = (Phantom)role;
                if (role.Player.Data != null && role.Player.Data.Disconnected) return;
                var caught = phantom.Caught;
                if (!caught)
                {
                    phantom.Fade();
                }
                else if (phantom.Faded)
                {
                    Unmorph(phantom.Player);
                    phantom.Player.myRend().color = Color.white;
                    phantom.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    phantom.Faded = false;
                    phantom.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}