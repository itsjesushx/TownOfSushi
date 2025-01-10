using UnityEngine.UI;

namespace TownOfSushi.Patches
{
    [HarmonyPatch(typeof(GameData))]
    public class DisconnectHandler
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (MeetingHud.Instance)
            {
                PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(x => x.TargetPlayerId == player.PlayerId);

                if (!player.Data.IsDead)
                {
                    if (voteArea == null) return;
                    if (voteArea.DidVote) voteArea.UnsetVote();
                    voteArea.AmDead = true;
                    voteArea.Overlay.gameObject.SetActive(true);
                    voteArea.Overlay.color = Color.white;
                    voteArea.XMark.gameObject.SetActive(true);
                    voteArea.XMark.transform.localScale = Vector3.one;
                }

                var blackmailers = AllRoles.Where(x => x.RoleType == RoleEnum.Blackmailer && x.Player != null).Cast<Blackmailer>();
                foreach (var role in blackmailers)
                {
                    if (role.Blackmailed != null && voteArea.TargetPlayerId == role.Blackmailed.PlayerId)
                    {
                        if (BlackmailMeetingUpdate.PrevXMark != null && BlackmailMeetingUpdate.PrevOverlay != null)
                        {
                            voteArea.XMark.sprite = BlackmailMeetingUpdate.PrevXMark;
                            voteArea.Overlay.sprite = BlackmailMeetingUpdate.PrevOverlay;
                            voteArea.XMark.transform.localPosition = new Vector3(
                                voteArea.XMark.transform.localPosition.x - BlackmailMeetingUpdate.LetterXOffset,
                                voteArea.XMark.transform.localPosition.y - BlackmailMeetingUpdate.LetterYOffset,
                                voteArea.XMark.transform.localPosition.z);
                        }
                    }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var swapper = GetRole<Swapper>(PlayerControl.LocalPlayer);
                    var button = swapper.Buttons[voteArea.TargetPlayerId];
                    if (button.GetComponent<SpriteRenderer>().sprite == TownOfSushi.SwapperSwitch)
                    {
                        swapper.ListOfActives[voteArea.TargetPlayerId] = false;
                        if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                        if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                        StartRPC(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    }
                    button.SetActive(false);
                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    swapper.Buttons[voteArea.TargetPlayerId] = null;
                }

                if (AddButtonVigilante.vigilanteUI != null) AddButtonVigilante.vigilanteUIExitButton.OnClick.Invoke();
                if (AssassinAddButton.assassinUI != null) AssassinAddButton.assassinUIExitButton.OnClick.Invoke();
                if (AddButtonDoomsayer.doomsayerUI != null) AddButtonDoomsayer.doomsayerUIExitButton.OnClick.Invoke();

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Jailor) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var jailor = GetRole<Jailor>(PlayerControl.LocalPlayer);
                    jailor.ExecuteButton.Destroy();
                    jailor.UsesText.Destroy();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Imitator) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    var imitatorRole = GetRole<Imitator>(PlayerControl.LocalPlayer);
                    var button = imitatorRole.Buttons[voteArea.TargetPlayerId];
                    if (button.GetComponent<SpriteRenderer>().sprite == TownOfSushi.ImitateSelectSprite)
                    {
                        imitatorRole.ListOfActives[voteArea.TargetPlayerId] = false;
                        if (SetImitate.Imitate == voteArea) SetImitate.Imitate = null;
                    }
                    button.SetActive(false);
                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    imitatorRole.Buttons[voteArea.TargetPlayerId] = null;
                }
            }
        }
    }
}