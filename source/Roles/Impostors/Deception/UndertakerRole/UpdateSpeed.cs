namespace TownOfSushi.Roles.Impostors.Deception.UndertakerRole
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Undertaker))
            {
                var role = GetRole<Undertaker>(__instance.myPlayer);
                if (role.CurrentlyDragging != null)
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                        __instance.body.velocity *= CustomGameOptions.UndertakerDragSpeed;
            }
        }
    }
}