namespace TownOfSushi.Roles.Abilities.AbilityMod.ButtonBarryAbility
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(AbilityEnum.ButtonBarry)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) return true;

            var role = GetAbility<ButtonBarry>(PlayerControl.LocalPlayer);
            if (__instance != role.ButtonButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ButtonUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (PlayerControl.LocalPlayer.RemainingEmergencies <= 0) return false;
            if (!__instance.enabled) return false;

            role.ButtonUsed = true;

            Rpc(CustomRPC.BarryButton, PlayerControl.LocalPlayer.PlayerId);

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(
                    MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                if (GameManager.Instance.CheckTaskCompletion()) return false;
                DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.RpcStartMeeting(null);
            }

            return false;
        }
    }
}