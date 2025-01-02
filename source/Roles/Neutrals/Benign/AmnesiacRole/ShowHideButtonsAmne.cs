using UnityEngine.UI;

namespace TownOfSushi.Roles.Neutral.Benign.AmnesiacRole
{
    public class ShowHideButtonsAmnesiac
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) return true;
                var Amnesiac = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
                foreach (var button in Amnesiac.Buttons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AddButtonAmnesiac.DisabledSprite)
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }

                if (Amnesiac.ListOfActives.Count(x => x) == 1)
                {
                    for (var i = 0; i < Amnesiac.ListOfActives.Count; i++)
                    {
                        if (!Amnesiac.ListOfActives[i]) continue;
                        SetRemember.Remember = __instance.playerStates[i];
                    }
                }

                return true;
            }
        }
    }
}