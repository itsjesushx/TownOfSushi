using UnityEngine.UI;

namespace TownOfSushi.Roles.Crewmates.Support.ImitatorRole
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonImitator
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfSushi.ImitateSelectSprite;
        public static Sprite DisabledSprite => TownOfSushi.ImitateDeselectSprite;
        public static void GenButton(Imitator role, int index, bool isDead)
        {
            if (!isDead)
            {
                role.Buttons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
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


        private static Action SetActive(Imitator role, int index)
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

                SetImitate.Imitate = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;
                    SetImitate.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Imitator))
            {
                var imitator = (Imitator)role;
                imitator.ListOfActives.Clear();
                imitator.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var imitatorRole = GetRole<Imitator>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        var imitatable = false;
                        var imitatedRole = GetPlayerRole(player).RoleType;
                        if (imitatedRole == RoleEnum.Haunter)
                        {
                            var haunter = GetRole<Haunter>(player);
                            imitatedRole = haunter.formerRole;
                        }
                        if (player.Data.IsDead && !player.Data.Disconnected && (
                            imitatedRole == RoleEnum.Investigator || 
                            imitatedRole == RoleEnum.Mystic ||
                            imitatedRole == RoleEnum.Seer   || 
                            imitatedRole == RoleEnum.Tracker || 
                            imitatedRole == RoleEnum.Vigilante ||
                            imitatedRole == RoleEnum.Veteran ||
                            imitatedRole == RoleEnum.Engineer || 
                            imitatedRole == RoleEnum.Medium ||
                            imitatedRole == RoleEnum.Transporter ||
                            imitatedRole == RoleEnum.Trapper ||
                            imitatedRole == RoleEnum.Medic ||
                            imitatedRole == RoleEnum.Oracle)) imitatable = true;
                        GenButton(imitatorRole, i, imitatable);
                    }
                }
            }
        }
    }
}