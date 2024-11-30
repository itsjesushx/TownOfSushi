namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    public class SpellMeetingUpdate
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHud_Update
        {
            public static void Postfix(MeetingHud __instance)
            {
                var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
                foreach (var role in witches)
                {
                    if (role.Spelled != null && !role.Spelled.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.Spelled.PlayerId);
                        playerState.NameText.text += " <color=#FF0000FF>†</color>";
                    }
                }
            }
        }
    }
}