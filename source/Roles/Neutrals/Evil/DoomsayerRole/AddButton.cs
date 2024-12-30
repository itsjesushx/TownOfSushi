namespace TownOfSushi.Roles.Neutral.Evil.DoomsayerRole
{
    [HarmonyPatch]
    public class AddButton
    {
        public static GameObject doomsayerUI;
        public static PassiveButton doomsayerUIExitButton;
        public static byte doomsayerCurrentTarget;
        public static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = PlayerById(voteArea.TargetPlayerId);
            if (player.IsJailed()) return true;
            if (
                    player == null ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            var role = GetPlayerRole(player);
            return role != null && role.Criteria();
        }

        public static void doomsayerOnClick(int buttonTarget, MeetingHud __instance) {
            var role = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (doomsayerUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

            Transform PhoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            Transform container = Object.Instantiate(PhoneUI, __instance.transform);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            doomsayerUI = container.gameObject;

            int i = 0;
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;

            doomsayerCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

            Transform exitButtonParent = (new GameObject()).transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = Object.Instantiate(buttonTemplate.transform, exitButtonParent);
            Transform exitButtonMask = Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            exitButtonParent.transform.localScale = new Vector3(0.217f, 0.9f, 1);
            doomsayerUIExitButton = exitButton.GetComponent<PassiveButton>();
            doomsayerUIExitButton.OnClick.RemoveAllListeners();
            doomsayerUIExitButton.OnClick.AddListener((System.Action)(() => {
                __instance.playerStates.ToList().ForEach(x => {
                    x.gameObject.SetActive(true);
                    if (PlayerControl.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null) Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });
                Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new List<Transform>();
            Transform selectedButton = null;

            foreach (var pair in role.SortedColorMapping) {
                Transform buttonParent = (new GameObject()).transform;
                buttonParent.SetParent(container);
                Transform button = Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = Object.Instantiate(maskTemplate, buttonParent);
                TMPro.TextMeshPro label = Object.Instantiate(textTemplate, button);
                button.GetComponent<SpriteRenderer>().sprite = ShipStatus.Instance.CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
                buttons.Add(button);
                int row = i/5, col = i%5;
                buttonParent.localPosition = new Vector3(-3.47f + 1.75f * col, 1.5f - 0.45f * row, -5);
                buttonParent.localScale = new Vector3(0.55f, 0.55f, 1f);
                label.text = ColorString(pair.Value, pair.Key);
                label.alignment = TMPro.TextAlignmentOptions.Center;
                label.transform.localPosition = new Vector3(0, 0, label.transform.localPosition.z);
                label.transform.localScale *= 1.7f;
                int copiedIndex = i;

                button.GetComponent<PassiveButton>().OnClick.RemoveAllListeners();
                if (!PlayerControl.LocalPlayer.Data.IsDead && !PlayerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId).Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() => {
                    if (selectedButton != button) {
                        selectedButton = button;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    } else {
                        PlayerControl focusedTarget = PlayerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || focusedTarget == null) return;

                        var mainRoleInfo = GetPlayerRole(focusedTarget);
                        var modRoleInfo = GetModifier(focusedTarget);

                        PlayerControl dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        if (modRoleInfo != null)
                            dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        
                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        Object.Destroy(container.gameObject);
                        __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                        // Shoot player
                        if (dyingTarget == focusedTarget)
                        {
                            DoomsayerKill.RpcMurderPlayer(role, dyingTarget, PlayerControl.LocalPlayer);
                        }
                        else
                        {
                            __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                            Flash(role.Color, 2.5f);
                        }
                    }
                }));
                i++;
            }
            container.transform.localScale *= 0.75f;
        }
    }
}