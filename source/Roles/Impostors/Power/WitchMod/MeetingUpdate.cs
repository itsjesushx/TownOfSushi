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
            public static Sprite Overlay => TownOfSushi.WitchOverlay;
            public static void Postfix(MeetingHud __instance)
            {
                var witches = AllRoles.Where(x => x.RoleType == RoleEnum.Witch && x.Player != null).Cast<Witch>();
                foreach (var role in witches)
                {
                    if (role.Spelled != null && !role.Spelled.Data.IsDead)
                    {
                        var playerState = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == role.Spelled.PlayerId);
                        playerState.Overlay.gameObject.SetActive(true);
                        if (PrevOverlay == null) PrevOverlay = playerState.Overlay.sprite;
                        playerState.Overlay.sprite = Overlay;
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