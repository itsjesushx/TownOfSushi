using System.Collections;
using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Swapper : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public Swapper(PlayerControl player) : base(player)
        {
            var CanButton = CustomGameOptions.SwapperButton ? "can call an emergency meeting." : "can't call an emergency meeting.";
            Name = "Swapper";
            StartText = () => "Swap the votes of two people";
            TaskText = () => "Swap two people's votes to save the Crew!";
            RoleInfo = $"You can swap the votes of two players, turning the tide of a vote to save the Crew lives or expose the killers. Your influence in the voting process is invaluable to the crew’s survival, but it requires careful strategy and timing. The Swapper {CanButton}";
            LoreText = "A master of subtle manipulation, you have the power to change the course of votes and sway the outcome of critical decisions. As the Swapper, you can swap the votes of two players, turning the tide of a vote to save innocent lives or expose the killers. Your influence in the voting process is invaluable to the crew’s survival, but it requires careful strategy and timing.";
            Color = Colors.Swapper;
            RoleType = RoleEnum.Swapper;
            RoleAlignment = RoleAlignment.CrewSupport;
            AddToRoleHistory(RoleType);
        }
    }

    [HarmonyPatch(typeof(MeetingHud))]
    public class SwapVotes
    {
        public static PlayerVoteArea Swap1;
        public static PlayerVoteArea Swap2;

        private static IEnumerator Slide2D(Transform target, Vector2 source, Vector2 dest, float duration = 0.75f)
        {
            var temp = default(Vector3);
            temp.z = target.position.z;
            for (var time = 0f; time < duration; time += Time.deltaTime)
            {
                var t = time / duration;
                temp.x = Mathf.SmoothStep(source.x, dest.x, t);
                temp.y = Mathf.SmoothStep(source.y, dest.y, t);
                target.position = temp;
                yield return null;
            }

            temp.x = dest.x;
            temp.y = dest.y;
            target.position = temp;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
        public static class SwapperVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                PluginSingleton<TownOfSushi>.Instance.Log.LogMessage(Swap1 == null ? "null" : Swap1.ToString());
                PluginSingleton<TownOfSushi>.Instance.Log.LogMessage(Swap2 == null ? "null" : Swap2.ToString());

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper))
                {
                    var swapper = GetRole<Swapper>(PlayerControl.LocalPlayer);
                    foreach (var button in swapper.Buttons.Where(button => button != null)) button.SetActive(false);
                }
                
                if (Swap1 == null || Swap2 == null) return;
                foreach (var swapper in AllRoles.Where(x => x.RoleType == RoleEnum.Swapper))
                {
                    if (swapper.Player.Data.IsDead || swapper.Player.Data.Disconnected) return;
                }
                PlayerControl swapPlayer1 = null;
                PlayerControl swapPlayer2 = null;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == Swap1.TargetPlayerId) swapPlayer1 = player;
                    if (player.PlayerId == Swap2.TargetPlayerId) swapPlayer2 = player;
                }
                if (swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected ||
                    swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected) return;

                var pool1 = Swap1.PlayerIcon.transform;
                var name1 = Swap1.NameText.transform;
                var background1 = Swap1.Background.transform;
                var mask1 = Swap1.MaskArea.transform;

                List<Transform> votes1 = new List<Transform>();
                for (var childI = 0; childI < Swap1.transform.childCount; childI++)
                    if (Swap1.transform.GetChild(childI).gameObject.name == "playerVote(Clone)") votes1.Add(Swap1.transform.GetChild(childI));

                var whiteBackground1 = Swap1.PlayerButton.transform;
                
                var pooldest1 = (Vector2) pool1.position;
                var namedest1 = (Vector2) name1.position;
                var backgroundDest1 = (Vector2) background1.position;
                var whiteBackgroundDest1 = (Vector2) whiteBackground1.position;
                var maskdest1 = (Vector2)mask1.position;

             //   background1.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

                var pool2 = Swap2.PlayerIcon.transform;
                var name2 = Swap2.NameText.transform;
                var background2 = Swap2.Background.transform;
                var mask2 = Swap2.MaskArea.transform;

                List<Transform> votes2 = new List<Transform>();
                for (var childI = 0; childI < Swap1.transform.childCount; childI++)
                    if (Swap1.transform.GetChild(childI).gameObject.name == "playerVote(Clone)") votes2.Add(Swap1.transform.GetChild(childI));

                var whiteBackground2 = Swap2.PlayerButton.transform;

                var pooldest2 = (Vector2) pool2.position;
                var namedest2 = (Vector2) name2.position;
                var backgrounddest2 = (Vector2) background2.position;
                var maskdest2 = (Vector2)mask2.position;

                var whiteBackgroundDest2 = (Vector2) whiteBackground2.position;

                foreach (var vote in votes2)
                {
                    vote.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, Swap1.MaskLayer);
                }
                foreach (var vote in votes1)
                {
                    vote.GetComponent<SpriteRenderer>().material.SetInt(PlayerMaterial.MaskLayer, Swap2.MaskLayer);
                }

                Coroutines.Start(Slide2D(pool1, pooldest1, pooldest2, 2f));
                Coroutines.Start(Slide2D(pool2, pooldest2, pooldest1, 2f));
                Coroutines.Start(Slide2D(name1, namedest1, namedest2, 2f));
                Coroutines.Start(Slide2D(name2, namedest2, namedest1, 2f));
                Coroutines.Start(Slide2D(mask1, maskdest1, maskdest2, 2f));
                Coroutines.Start(Slide2D(mask2, maskdest2, maskdest1, 2f));
                Coroutines.Start(Slide2D(whiteBackground1, whiteBackgroundDest1, whiteBackgroundDest2, 2f));
                Coroutines.Start(Slide2D(whiteBackground2, whiteBackgroundDest2, whiteBackgroundDest1, 2f));
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_StartSwapper
        {
            public static void Postfix(MeetingHud __instance)
            {
                Swap1 = null;
                Swap2 = null;
            }
        }
    }
    public class ShowHideButtonsSwapper
    {
        public static Dictionary<byte, int> CalculateVotes(MeetingHud __instance)
        {
            var self = CalculateVotesSwap(__instance);

            var maxIdx = self.MaxPair(out var tie);

            var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == maxIdx.Key);

            return self;
        }
        public static Dictionary<byte, int> CalculateVotesSwap(MeetingHud __instance)
        {
            var self = RegisterExtraVotes.CalculateAllVotes(__instance);
            if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null) return self;
            foreach (var swapper in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Swapper))
            {
                if (swapper.Player.Data.IsDead || swapper.Player.Data.Disconnected) return self;
            }
            PlayerControl swapPlayer1 = null;
            PlayerControl swapPlayer2 = null;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == SwapVotes.Swap1.TargetPlayerId) swapPlayer1 = player;
                if (player.PlayerId == SwapVotes.Swap2.TargetPlayerId) swapPlayer2 = player;
            }
            if (swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected ||
                swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected) return self;

            var swap1 = 0;
            if (self.TryGetValue(SwapVotes.Swap1.TargetPlayerId, out var value)) swap1 = value;

            var swap2 = 0;
            if (self.TryGetValue(SwapVotes.Swap2.TargetPlayerId, out var value2)) swap2 = value2;

            self[SwapVotes.Swap2.TargetPlayerId] = swap1;
            self[SwapVotes.Swap1.TargetPlayerId] = swap2;

            return self;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) return true;
                var swapper = GetRole<Swapper>(PlayerControl.LocalPlayer);
                foreach (var button in swapper.Buttons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AddButtonSwapper.DisabledSprite)
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }

                if (swapper.ListOfActives.Count(x => x) == 2)
                {
                    var toSet1 = true;
                    for (var i = 0; i < swapper.ListOfActives.Count; i++)
                    {
                        if (!swapper.ListOfActives[i]) continue;

                        if (toSet1)
                        {
                            SwapVotes.Swap1 = __instance.playerStates[i];
                            toSet1 = false;
                        }
                        else
                        {
                            SwapVotes.Swap2 = __instance.playerStates[i];
                        }
                    }
                }

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null) return true;

                Utils.Rpc(CustomRPC.SetSwaps, SwapVotes.Swap1.TargetPlayerId, SwapVotes.Swap2.TargetPlayerId);
                return true;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.CheckForEndVoting))]
        public static class CheckForEndVoting
        {
            private static bool CheckVoted(PlayerVoteArea playerVoteArea)
            {
                if (playerVoteArea.AmDead || playerVoteArea.DidVote)
                    return true;

                var playerInfo = GameData.Instance.GetPlayerById(playerVoteArea.TargetPlayerId);
                if (playerInfo == null)
                    return true;

                var playerControl = playerInfo.Object;

                if (playerControl.Is(AbilityEnum.Assassin) && playerInfo.IsDead)
                {
                    playerVoteArea.VotedFor = PlayerVoteArea.DeadVote;
                    playerVoteArea.SetDead(false, true);
                    return true;
                }

                return true;
            }
            public static bool Prefix(MeetingHud __instance)
            {
                if (__instance.playerStates.All(ps => ps.AmDead || ps.DidVote && CheckVoted(ps)))
                {
                    var self = CalculateVotes(__instance);

                    var array = new Il2CppStructArray<MeetingHud.VoterState>(__instance.playerStates.Length);

                    var maxIdx = self.MaxPair(out var tie);

                    var exiled = GameData.Instance.AllPlayers.ToArray().FirstOrDefault(v => !tie && v.PlayerId == maxIdx.Key);
                    for (var i = 0; i < __instance.playerStates.Length; i++)
                    {
                        var playerVoteArea = __instance.playerStates[i];
                        array[i] = new MeetingHud.VoterState
                        {
                            VoterId = playerVoteArea.TargetPlayerId,
                            VotedForId = playerVoteArea.VotedFor
                        };
                    }

                    __instance.RpcVotingComplete(array, exiled, tie);
                }

                return false;
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonSwapper
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfSushi.SwapperSwitch;
        public static Sprite DisabledSprite => TownOfSushi.SwapperSwitchDisabled;
        public static void GenButton(Swapper role, int index, bool isDead)
        {
            if (isDead)
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

            passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            role.Buttons.Add(newButton);
            role.ListOfActives.Add(false);
        }


        private static Action SetActive(Swapper role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 2 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite) return;

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index] ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = !role.ListOfActives[index];

                _mostRecentId = index;

                SwapVotes.Swap1 = null;
                SwapVotes.Swap2 = null;
                var toSet1 = true;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;

                    if (toSet1)
                    {
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates[i];
                        toSet1 = false;
                    }
                    else
                    {
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates[i];
                    }
                }

                if (SwapVotes.Swap1 == null || SwapVotes.Swap2 == null)
                {
                    Utils.Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    return;
                }

                Utils.Rpc(CustomRPC.SetSwaps, SwapVotes.Swap1.TargetPlayerId, SwapVotes.Swap2.TargetPlayerId);
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Swapper))
            {
                var swapper = (Swapper) role;
                swapper.ListOfActives.Clear();
                swapper.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Swapper)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var swapperrole = GetRole<Swapper>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
                GenButton(swapperrole, i, __instance.playerStates[i].AmDead || Utils.PlayerById(__instance.playerStates[i].TargetPlayerId).IsJailed());
        }
    }
}