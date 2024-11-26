
using UnityEngine.UI;

namespace TownOfSushi.Roles.Crewmates.Support.ImitatorRole
{
    public class ShowHideButtonsImitator
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return true;
                var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
                foreach (var button in imitator.Buttons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AddButtonImitator.DisabledSprite)
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }

                if (imitator.ListOfActives.Count(x => x) == 1)
                {
                    for (var i = 0; i < imitator.ListOfActives.Count; i++)
                    {
                        if (!imitator.ListOfActives[i]) continue;
                        SetImitate.Imitate = __instance.playerStates[i];
                    }
                }

                return true;
            }
        }
    }
}