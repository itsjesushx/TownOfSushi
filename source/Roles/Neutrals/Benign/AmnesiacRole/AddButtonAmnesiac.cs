using UnityEngine.UI;

namespace TownOfSushi.Roles.Neutral.Benign.AmnesiacRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonAmnesiac
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfSushi.ImitateSelectSprite;
        public static Sprite DisabledSprite => TownOfSushi.ImitateDeselectSprite;
        private static List<GameObject> buttonPool = new List<GameObject>();
        public static void GenButton(Amnesiac role, int index, bool isDead)
        {
            if (role.Remembered) return;
            if (!isDead)
            {
                role.Buttons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            GameObject newButton;
            if (buttonPool.Count > 0)
            {
                newButton = buttonPool[0];
                buttonPool.RemoveAt(0);
                newButton.SetActive(true);
            }
            else
            {
                newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            }

            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;
            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.Buttons.Add(newButton);
            role.ListOfActives.Add(false);
        }

        public static void ReturnButtonToPool(GameObject button)
        {
            button.SetActive(false);
            buttonPool.Add(button);
        }

        private static Action SetActive(Amnesiac role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 1 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                {
                    int active = 0;
                    for (var i = 0; i < role.ListOfActives.Count; i++) if (role.ListOfActives[i]) active = i;

                    role.Buttons[active].GetComponent<SpriteRenderer>().sprite =
                        role.ListOfActives[active] ? DisabledSprite : ActiveSprite;

                    role.ListOfActives[active] = !role.ListOfActives[active];
                }

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index] ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = !role.ListOfActives[index];

                _mostRecentId = index;

                SetRemember.Remember = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;
                    SetRemember.Remember = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Amnesiac))
            {
                var Amnesiac = (Amnesiac)role;
                Amnesiac.ListOfActives.Clear();
                Amnesiac.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var AmnesiacRole = GetRole<Amnesiac>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        var stealable = false;
                        var StolenRole = GetPlayerRole(player).RoleType;
                        if (player.Data.IsDead && !player.Data.Disconnected && AmnesiacRole.RolesToRemember.Contains(StolenRole)) stealable = true;
                        GenButton(AmnesiacRole, i, stealable);
                    }
                }
            }
        }
    }
}