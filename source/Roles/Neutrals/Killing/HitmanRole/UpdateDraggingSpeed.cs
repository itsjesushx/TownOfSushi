namespace TownOfSushi.Roles.Neutral.Killing.HitmanRole
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
    public static class PlayerPhysics_FixedUpdate
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (__instance.myPlayer.Is(RoleEnum.Hitman))
            {
                var role = GetRole<Hitman>(__instance.myPlayer);
                if (role.CurrentlyDragging != null)
                    if (__instance.AmOwner && GameData.Instance && __instance.myPlayer.CanMove)
                        __instance.body.velocity *= CustomGameOptions.HitmanDragSpeed;
            }
        }
    }
}