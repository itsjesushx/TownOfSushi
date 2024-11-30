namespace TownOfSushi.Roles.Impostors.Power.WitchRole
{
    public class SpellMeetingUpdate
    {
        public static bool shookAlready = false;
        public static Sprite PrevOverlay = null;
        public const float LetterXOffset = 0.22f;
        public static Sprite PrevXMark = null;
        public const float LetterYOffset = -0.32f;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void Postfix(MeetingHud __instance)
            {
                shookAlready = false;
            }
        }

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
                        playerState.Overlay.gameObject.SetActive(true);
                        //playerState.NameText.text += " <color=#FF0000FF>Cursed</color>";
                        if (__instance.state != MeetingHud.VoteStates.Animating && shookAlready == false)
                        {
                            shookAlready = true;
                            (__instance as MonoBehaviour).StartCoroutine(Effects.SwayX(playerState.transform));
                        }
                    }
                }
            }
        }
    }
}