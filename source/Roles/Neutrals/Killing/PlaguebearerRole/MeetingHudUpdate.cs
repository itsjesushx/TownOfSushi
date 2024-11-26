



namespace TownOfSushi.Roles.Neutral.Killing.PlaguebearerRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = GetPlayerRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Plaguebearer) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Plaguebearer)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.InfectedPlayers.Remove(targetId);
                    continue;
                }
                if (role.InfectedPlayers.Contains(targetId) && role.Player.PlayerId != targetId) state.NameText.text += "<color=#E6FFB3FF> [♨]</color>";
            }
        }
    }
}