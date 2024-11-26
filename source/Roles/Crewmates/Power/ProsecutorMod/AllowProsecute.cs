namespace TownOfSushi.Roles.Crewmates.Power.ProsecutorRole
{
    [HarmonyPatch(typeof(PlayerVoteArea))]
    public class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Prosecutor)) return true;
                var role = GetRole<Prosecutor>(PlayerControl.LocalPlayer);
                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                    __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != role.Prosecute)
                {
                    if (role.StartProsecute)
                    {
                        role.ProsecuteThisMeeting = true;
                        role.StartProsecute = false;
                        Rpc(CustomRPC.Prosecute, false, role.Player.PlayerId);
                    }
                    return true;
                }
                else
                {
                    role.StartProsecute = true;
                    MeetingHud.Instance.SkipVoteButton.gameObject.SetActive(false);
                    AddProsecute.UpdateButton(role, MeetingHud.Instance);
                    if (!AmongUsClient.Instance.AmHost)
                    {
                        Rpc(CustomRPC.Prosecute, true, role.Player.PlayerId);
                    }
                    return false;
                }
            }
        }
    }
}