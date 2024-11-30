using TownOfSushi.Roles.Crewmates.Support.MedicRole;

namespace TownOfSushi.Roles.Crewmates.Investigative.MysticMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    internal class BodyReportPatch
    {
        private static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo info)
        {
            if (info == null) return;
            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == info.PlayerId).ToArray();
            DeadPlayer killer = null;

            if (matches.Length > 0)
                killer = matches[0];

            if (killer == null)
                return;

            var isMysticAlive = __instance.Is(RoleEnum.Mystic);
            var areReportsEnabled = CustomGameOptions.MysticReportOn;

            if (!isMysticAlive || !areReportsEnabled)
                return;

            var isUserMystic = PlayerControl.LocalPlayer.Is(RoleEnum.Mystic);
            if (!isUserMystic)
                return;
            var br = new BodyReport
            {
                Killer = PlayerById(killer.KillerId),
                Reporter = __instance,
                Body = PlayerById(killer.PlayerId),
                KillAge = (float) (DateTime.UtcNow - killer.KillTime).TotalMilliseconds
            };

            var reportMsg = BodyReport.ParseBodyReport(br);

            if (string.IsNullOrWhiteSpace(reportMsg))
                return;

            if (DestroyableSingleton<HudManager>.Instance)
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, reportMsg);
        }
    }
}