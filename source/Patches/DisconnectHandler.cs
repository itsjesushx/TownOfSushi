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
            if (LocalPlayer().Is(RoleEnum.BountyHunter))
            {
                var bountyHunter = GetRole<BountyHunter>(LocalPlayer());
                if (bountyHunter.Bounty == player) bountyHunter.Bounty = bountyHunter.AddBounty(player);
            }
            if (Meeting())
            {
                PlayerVoteArea voteArea = Meeting().playerStates.First(x => x.TargetPlayerId == player.PlayerId);

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

                if (LocalPlayer().Is(RoleEnum.Swapper) && !IsDead())
                {
                    var swapper = GetRole<Swapper>(PlayerControl.LocalPlayer);
                    var index = int.MaxValue;
                    for (var i = 0; i < swapper.ListOfActives.Count; i++)
                    {
                        if (swapper.ListOfActives[i].Item1 == voteArea.TargetPlayerId)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != int.MaxValue)
                    {
                        var button = swapper.Buttons[index];
                        if (button != null)
                        {
                            if (button.GetComponent<SpriteRenderer>().sprite == TownOfSushi.SwapperSwitch)
                            {
                                swapper.ListOfActives[index] = (swapper.ListOfActives[index].Item1, false);
                                if (SwapVotes.Swap1 == voteArea) SwapVotes.Swap1 = null;
                                if (SwapVotes.Swap2 == voteArea) SwapVotes.Swap2 = null;
                                StartRPC(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                            }
                            button.SetActive(false);
                            button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            swapper.Buttons[index] = null;
                        }
                    }
                }
                

                if (AddButtonVigilante.vigilanteUI != null) AddButtonVigilante.vigilanteUIExitButton.OnClick.Invoke();
                if (AssassinAddButton.assassinUI != null) AssassinAddButton.assassinUIExitButton.OnClick.Invoke();
                if (AddButtonDoomsayer.doomsayerUI != null) AddButtonDoomsayer.doomsayerUIExitButton.OnClick.Invoke();

                if (LocalPlayer().Is(RoleEnum.Jailor) && !IsDead())
                {
                    var jailor = GetRole<Jailor>(LocalPlayer());
                    jailor.ExecuteButton.Destroy();
                    jailor.UsesText.Destroy();
                }

                if (LocalPlayer().Is(RoleEnum.Deputy) && !IsDead())
                {
                    var Deputy = GetRole<Deputy>(LocalPlayer());
                    Deputy.ExecuteButton.Destroy();
                }

                if (LocalPlayer().Is(RoleEnum.Imitator) && !IsDead())
                {
                    var imitatorRole = GetRole<Imitator>(LocalPlayer());
                    var index = int.MaxValue;
                    for (var i = 0; i < imitatorRole.ListOfActives.Count; i++)
                    {
                        if (imitatorRole.ListOfActives[i].Item1 == voteArea.TargetPlayerId)
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index != int.MaxValue)
                    {
                        var button = imitatorRole.Buttons[index];
                        if (button != null)
                        {
                            if (button.GetComponent<SpriteRenderer>().sprite == TownOfSushi.ImitateSelectSprite)
                            {
                                imitatorRole.ListOfActives[index] = (imitatorRole.ListOfActives[index].Item1, false);
                                if (SetImitate.Imitate == voteArea) SetImitate.Imitate = null;
                            }
                            button.SetActive(false);
                            button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            imitatorRole.Buttons[index] = null;
                        }
                    }
                }
            }
        }
    }
}