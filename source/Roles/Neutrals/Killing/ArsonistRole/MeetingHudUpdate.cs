namespace TownOfSushi.Roles.Neutral.Killing.ArsonistRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = GetPlayerRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Arsonist) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Arsonist)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.DousedPlayers.Remove(targetId);
                    continue;
                }
                if (role.DousedPlayers.Contains(targetId)) state.NameText.text += "<color=#FF4D00FF> [♨]</color>";
            }
        }
    }
}