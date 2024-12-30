namespace TownOfSushi.Roles.Crewmates.Killing.HunterRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = GetPlayerRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Hunter) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Hunter)_role;
            foreach (var state in __instance.playerStates)
            {
                var player = Utils.PlayerById(state.TargetPlayerId);
                var playerData = player?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.CaughtPlayers.Remove(player);
                    continue;
                }
                if (role.CaughtPlayers.Any(pc => pc.PlayerId == player.PlayerId)) state.NameText.color = Color.black;
            }
        }
    }
}