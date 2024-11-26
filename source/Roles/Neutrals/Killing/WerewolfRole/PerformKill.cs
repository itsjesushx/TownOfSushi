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
                var playerId = role.ClosestPlayer.PlayerId;

                if (interact.AbilityUsed)
                {
                    role.Maul(role.Player);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Maul, SendOption.Reliable, -1);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(playerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                
                if (interact.FullCooldownReset)
                    role.LastMauled = DateTime.UtcNow;
                else if (interact.GaReset)
                    role.LastMauled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }

            return false;
        }
    }
}