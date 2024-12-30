namespace TownOfSushi.Roles.Crewmates.Investigative.MysticMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            if (!CustomGameOptions.ExamineReportOn) return;
            var MysticRole = GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (MysticRole.LastExaminedPlayer != null)
            {
                var playerResults = BodyReport.PlayerReportFeedback(MysticRole.LastExaminedPlayer);
                var roleResults = BodyReport.RoleReportFeedback(MysticRole.LastExaminedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }
    }
}