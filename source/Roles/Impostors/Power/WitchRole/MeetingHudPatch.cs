namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    public class SpellMeetingUpdate
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHud_Update
        {
            public static void Postfix(MeetingHud __instance)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    foreach (var role in GetRoles(RoleEnum.Witch))
                    {
                        var witch = (Witch)role;
                        if (witch.Player.Data.IsDead) return;
                        if (witch.SpelledPlayers.Contains(player.PlayerId) && !player.Data.IsDead)
                        {
                            var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);
                            if (playerState != null)
                            {
                                playerState.NameText.text += " <color=#FF0000FF> [†]</color>";
                            }
                        }
                    }
                }
            }
        }
    }
}