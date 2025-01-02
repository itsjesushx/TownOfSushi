namespace TownOfSushi.Roles.Neutral.Benign.AmnesiacRole
{
    [HarmonyPatch(typeof(MeetingHud))]
    public class SetRemember
    {
        public static PlayerVoteArea Remember;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (Remember == null) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
                {
                    var Amnesiac = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
                    foreach (var button in Amnesiac.Buttons.Where(button => button != null)) button.SetActive(false);

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == Remember.TargetPlayerId) 
                        { 
                            Amnesiac.ToRemember = player;
                        }
                        if (Remember == null)
                        {
                            Rpc(CustomRPC.Remember, Amnesiac.Player.PlayerId, sbyte.MaxValue);
                            return;
                        }

                        Rpc(CustomRPC.Remember, Amnesiac.Player.PlayerId, player.PlayerId);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                Remember = null;
            }
        }
    }
}