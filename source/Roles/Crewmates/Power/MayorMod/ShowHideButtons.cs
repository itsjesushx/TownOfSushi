

namespace TownOfSushi.Roles.Crewmates.Power.MayorRole
{
    public class ShowHideButtonsMayor
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mayor)) return true;
                var mayor = GetRole<Mayor>(PlayerControl.LocalPlayer);
                if (!mayor.Revealed) mayor.RevealButton.Destroy();
                return true;
            }
        }
    }
}