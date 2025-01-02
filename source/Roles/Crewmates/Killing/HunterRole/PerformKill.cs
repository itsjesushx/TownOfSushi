namespace TownOfSushi.Roles.Crewmates.Killing.HunterRole
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class Stalk
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            var role = GetRole<Hunter>(PlayerControl.LocalPlayer);
            if (__instance == role.StalkButton)
            {
                if (role.ClosestStalkPlayer == null) return false;
                if (!role.StalkUsable) return false;
                if (role.StalkTimer() != 0) return false;
                var stalkInteract = Interact(PlayerControl.LocalPlayer, role.ClosestStalkPlayer, false);
                if (stalkInteract[3] == true)
                {
                    role.StalkDuration = CustomGameOptions.HunterStalkDuration;
                    role.StalkedPlayer = role.ClosestStalkPlayer;
                    role.MaxUses--;
                    role.Stalk();
                    Rpc(CustomRPC.HunterStalk, PlayerControl.LocalPlayer.PlayerId, role.ClosestStalkPlayer.PlayerId);
                }
                if (stalkInteract[0] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                }
                else if (stalkInteract[1] == true)
                {
                    role.LastStalked = DateTime.UtcNow;
                    role.LastStalked = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
                }
                return false;
            }

            if (role.ClosestPlayer == null) return false;
            if (!role.CaughtPlayers.Contains(role.ClosestPlayer)) return false;
            if (role.HunterKillTimer() != 0) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[0] == true)
            {
                role.LastKilled = DateTime.UtcNow;
            }
            else if (interact[1] == true)
            {
                role.LastKilled = DateTime.UtcNow;
                role.LastKilled = role.LastKilled.AddSeconds(-CustomGameOptions.HunterKillCd + CustomGameOptions.ProtectKCReset);
            }
            return false;
        }
    }
}