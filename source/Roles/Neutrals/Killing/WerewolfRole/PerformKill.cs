namespace TownOfSushi.Roles.Neutral.Killing.WerewolfRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformMaul
    {
        public static bool Prefix(KillButton __instance)
        {
            if (NoButton(PlayerControl.LocalPlayer, RoleEnum.Werewolf))
                return false;

            var role = GetRole<Werewolf>(PlayerControl.LocalPlayer);

            if (__instance == role.MaulButton)
            {
                if (!ButtonUsable(__instance))
                    return false;

                if (role.MaulTimer() != 0f)
                    return false;

                if (IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);

                if (interact[4] == true)
                {
                    role.Maul();
                    Rpc(CustomRPC.Maul, PlayerControl.LocalPlayer.PlayerId);
                }
                
                if (interact[0] == true)
                    role.LastMauled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }

            return false;
        }
    }
}