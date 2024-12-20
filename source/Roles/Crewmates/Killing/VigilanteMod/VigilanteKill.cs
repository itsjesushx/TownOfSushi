using UnityEngine.UI;
using TownOfSushi.Roles.Crewmates.Support.MedicRole;
using TownOfSushi.Roles.Impostors.Support.BlackmailerRole;
using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;

namespace TownOfSushi.Roles.Crewmates.Killing.VigilanteRole
{
    public class VigilanteKill
    {
        public static void RpcMurderPlayer(Vigilante vigilanteP, PlayerControl player, PlayerControl vigilante)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(vigilanteP, voteArea, player, vigilante);
        }
        public static void RpcMurderPlayer(Vigilante vigilanteP, PlayerVoteArea voteArea, PlayerControl player, PlayerControl vigilante)
        {
            MurderPlayer(vigilanteP, voteArea, player);
            VigiKillCount(player, vigilante);
            Utils.Rpc(CustomRPC.VigilanteKill, player.PlayerId, vigilante.PlayerId);
        }

        public static void MurderPlayer(Vigilante vigilanteP, PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(vigilanteP, voteArea, player);
        }
        public static void VigiKillCount(PlayerControl player, PlayerControl vigilante)
        {
            var vigi = GetRole<Vigilante>(vigilante);
            if (player == vigilante) vigi.IncorrectAssassinKills += 1;
            else vigi.CorrectAssassinKills += 1;
        }
        public static void MurderPlayer(
            Vigilante vigilanteP,
            PlayerVoteArea voteArea,
            PlayerControl player)
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var vigilantePlayer = vigilanteP.Player;

                try
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}
                if (PlayerControl.LocalPlayer == player) {
                    hudManager.KillOverlay.ShowKillAnimation(vigilantePlayer.Data, player.Data);
                    if (AddButton.vigilanteUI != null) AddButton.vigilanteUIExitButton.OnClick.Invoke();
                    if (Abilities.AbilityMod.AssassinAbility.AddButton.assassinUI != null) Abilities.AbilityMod.AssassinAbility.AddButton.assassinUIExitButton.OnClick.Invoke();
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
            }
            player.Die(DeathReason.Kill, false);
            
            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Guessed;
            role2.KilledBy = " By " + Utils.ColorString(Colors.Vigilante, vigilanteP.PlayerName);


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