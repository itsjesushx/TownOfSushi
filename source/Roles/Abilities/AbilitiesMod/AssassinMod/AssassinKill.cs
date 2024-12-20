using UnityEngine.UI;
using TownOfSushi.Roles.Crewmates.Support.MedicRole;
using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;
using TownOfSushi.Roles.Impostors.Support.BlackmailerRole;

namespace TownOfSushi.Roles.Abilities.AbilityMod.AssassinAbility
{
    public class AssassinKill
    {
        public static void RpcMurderPlayer(Assassin assassinP, PlayerControl player, PlayerControl assassin)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(assassinP, voteArea, player,  assassin);
        }
        public static void RpcMurderPlayer(Assassin assassinP, PlayerVoteArea voteArea, PlayerControl player, PlayerControl assassin)
        {
            MurderPlayer(assassinP, voteArea, player);
            AssassinKillCount(player, assassin);
            Rpc(CustomRPC.AssassinKill, player.PlayerId, assassin.PlayerId);
        }

        public static void MurderPlayer(Assassin assassinP, PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(assassinP, voteArea, player);
        }
        public static void AssassinKillCount(PlayerControl player, PlayerControl assassin)
        {
            var assassinPlayer = GetPlayerRole(assassin);
            if (player == assassin) assassinPlayer.IncorrectAssassinKills += 1;
            else assassinPlayer.CorrectAssassinKills += 1;
        }
        public static void MurderPlayer(
            Assassin assassinP, 
            PlayerVoteArea voteArea,
            PlayerControl player
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var assassinPlayer = assassinP.Player;
            try
            {
                SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
            } 
            catch {}
                if (PlayerControl.LocalPlayer == player) 
                {
                    hudManager.KillOverlay.ShowKillAnimation(assassinPlayer.Data, player.Data);
                    if (Crewmates.Killing.VigilanteRole.AddButton.vigilanteUI != null) Roles.Crewmates.Killing.VigilanteRole.AddButton.vigilanteUIExitButton.OnClick.Invoke();
                    if (AddButton.assassinUI != null) AddButton.assassinUIExitButton.OnClick.Invoke();
                    if (Neutral.Evil.DoomsayerRole.AddButton.doomsayerUI != null) Neutral.Evil.DoomsayerRole.AddButton.doomsayerUIExitButton.OnClick.Invoke();
            }
            

            var amOwner = player.AmOwner;
            if (amOwner)
            {
                //ShowDeadBodies = true;
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0;i < player.myTasks.Count;i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        UnityEngine.Object.Destroy(playerTask.gameObject);
                    }

                    player.myTasks.Clear();
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostIgnoreTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0)
                    );
                }
                else
                {
                    importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                        StringNames.GhostDoTasks,
                        new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                }

                player.myTasks.Insert(0, importantTextTask);

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
                    imitator.ListOfActives.Clear();
                    imitator.Buttons.Clear();
                    SetImitate.Imitate = null;
                    var buttons = GetRole<Imitator>(player).Buttons;
                    foreach (var button in buttons)
                    {
                        button.SetActive(false);
                        button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                    }
                }
            }
            player.Die(DeathReason.Kill, false);

            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Guessed;
            role2.KilledBy = " By " + ColorString(Colors.Impostor, assassinP.PlayerName);

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = System.DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);
            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();
            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = MeetingHud.Instance;
            if (amOwner)
            {
                meetingHud.SetForegroundForDead();
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

            if (player.Is(RoleEnum.Mayor))
            {
                var mayor = GetRole<Mayor>(PlayerControl.LocalPlayer);
                mayor.RevealButton.Destroy();
            }
            
            if (player.Is(RoleEnum.Imitator))
            {
                var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
                imitator.ListOfActives.Clear();
                imitator.Buttons.Clear();
                SetImitate.Imitate = null;
                var buttons = GetRole<Imitator>(player).Buttons;
                foreach (var button in buttons)
                {
                    button.SetActive(false);
                    button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }
            }

            if (player.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
            }
            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
        }
    }
}
