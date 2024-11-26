namespace TownOfSushi.Roles.Crewmates.Power.ProsecutorRole
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            foreach (var pros in GetRoles(RoleEnum.Prosecutor))
            {
                var prosRole = (Prosecutor)pros;
                prosRole.StartProsecute = false;
            }
            return;
        }
    }
}
