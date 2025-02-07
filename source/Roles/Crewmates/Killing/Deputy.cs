using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Deputy : Role
    {
        public Deputy (PlayerControl player) : base(player)
        {
            Name = "Deputy";
            StartText = () => "Shoot the <color=#FF0000FF>Impostors</color>";
            TaskText = () => "Shoot and kill evildoers in meetings";
            RoleInfo = $"The Deputy can shoot a player in meetings, if the player is a non-crewmate they die, else the deputy loses the ability to execute again.";
            LoreText = "The Deputy serves as the Crewmates' last line of defense. Tasked with maintaining order and justice aboard the ship, the Deputy uses their sharp instincts to identify and eliminate threats during meetings. However, their duty is not without peril—one wrong decision could lead to their own downfall.";
            Color = ColorManager.Deputy;
            RoleAlignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Deputy;
            AddToRoleHistory(RoleType);
            RemainingKills = CustomGameOptions.DeputyKills;
        }
        public bool HasExectutedAlready { get; set; } = false;
        public int RemainingKills { get; set; }
        public GameObject ExecuteButton = new GameObject();
    }
    
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddExecuteButtons
    {
        public static void GenButton(Deputy role, int index)
        {
            var voteArea = Meeting().playerStates[index];
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = TownOfSushi.TargetIcon;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Execute(role, (byte)index));
            role.ExecuteButton = newButton;
        }
        private static Action Execute(Deputy role, byte targetPlayerId)
        {
            void Listener()
            {
               var executeTarget = PlayerById(targetPlayerId);

               if (executeTarget == null) return;
               if (executeTarget.Data.IsDead) return;
               if (role.RemainingKills <= 0) return;
               if (role.HasExectutedAlready) return;

               role.RemainingKills -= 1;
               if (executeTarget.Is(Faction.Crewmates))        
               {
                   role.IncorrectShots += 1;
                   role.RemainingKills = 0;
                   Flash(Color.red);
                   Sound().PlaySound(Ship().SabotageSound, false, 1f, null);
               }
               else
               {            
                   role.CorrectDeputyShot += 1;            
                   Flash(Color.green);        
               }
              ExecuteKill(role, executeTarget);
           StartRPC(CustomRPC.ExecuteDeputyKill, role.Player.PlayerId, targetPlayerId);
            }
            return Listener;
        }

        public static void ExecuteKill (Deputy deputy, PlayerControl player)
        {
            var voteArea = Meeting().playerStates .FirstOrDefault(x => x.TargetPlayerId == player.PlayerId);

            if (player == null || voteArea == null) return;

            var hudManager = HUDManager();

               try
                {
                    Sound().PlaySound(LocalPlayer().KillSfx, false, 1f);
                } 
                catch {}

                if (LocalPlayer()== player) 
                {
                    hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
                    if (AddButtonVigilante.vigilanteUI != null) AddButtonVigilante.vigilanteUIExitButton.OnClick.Invoke();
                    if (AssassinAddButton.assassinUI != null) AssassinAddButton.assassinUIExitButton.OnClick.Invoke();
                    if (AddButtonDoomsayer.doomsayerUI != null) AddButtonDoomsayer.doomsayerUIExitButton.OnClick.Invoke();
                }
            
            var amOwner = player.AmOwner;
            if (amOwner)
            {
                hudManager.ShadowQuad.gameObject.SetActive(false);
                player.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                player.RpcSetScanner(false);
                ImportantTextTask importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                if (!OptionsManager().currentNormalGameOptions.GhostsDoTasks)
                {
                    for (int i = 0; i < player.myTasks.Count; i++)
                    {
                        PlayerTask playerTask = player.myTasks.ToArray()[i];
                        playerTask.OnRemove();
                        Object.Destroy(playerTask.gameObject);
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
                        var imitator = GetRole<Imitator>(LocalPlayer());
                        var buttons = GetRole<Imitator>(player).Buttons;
                        foreach (var button in buttons)
                        {
                            if (button != null)
                            {
                                button.SetActive(false);
                                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            }
                        }
                        imitator.ListOfActives.Clear();
                        imitator.Buttons.Clear();
                        SetImitate.Imitate = null;
                    }

                    if (player.Is(RoleEnum.Swapper))
                    {
                        var swapper = GetRole<Swapper>(LocalPlayer());
                        var buttons = GetRole<Swapper>(player).Buttons;
                        foreach (var button in buttons)
                        {
                            if (button != null)
                            {
                                button.SetActive(false);
                                button.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                            }
                        }
                        swapper.ListOfActives.Clear();
                        swapper.Buttons.Clear();
                        SwapVotes.Swap1 = null;
                        SwapVotes.Swap2 = null;
                        StartRPC(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                    }

                    if (player.Is(RoleEnum.Jailor))
                    {
                        var jailor = GetRole<Jailor>(LocalPlayer());
                        jailor.ExecuteButton.Destroy();
                        jailor.UsesText.Destroy();
                    }

                    if (player.Is(RoleEnum.Deputy))
                    {
                        var Deputy = GetRole<Deputy>(LocalPlayer());
                        Deputy.ExecuteButton.Destroy();
                    }
                
            }
            player.Die(DeathReason.Kill, false);

            GameHistory.CreateDeathReason(player, CustomDeathReason.ExecutedByDeputy, deputy.Player);

            var deadPlayer = new DeadPlayer
            {
                PlayerId = player.PlayerId,
                KillerId = player.PlayerId,
                KillTime = DateTime.UtcNow,
            };

            Murder.KilledPlayers.Add(deadPlayer);

            if (voteArea == null) return;
            if (voteArea.DidVote) voteArea.UnsetVote();

            voteArea.AmDead = true;
            voteArea.Overlay.gameObject.SetActive(true);
            voteArea.Overlay.color = Color.white;
            voteArea.XMark.gameObject.SetActive(true);
            voteArea.XMark.transform.localScale = Vector3.one;

            var meetingHud = Meeting();
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

            if (player.Is(RoleEnum.Imitator) && !player.Data.IsDead)
            {
                var imitatorRole = GetRole<Imitator>(player);
                if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                {
                    AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                }
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AssassinExileControllerPatch.AssassinatedPlayers.Add(player);
            deputy.HasExectutedAlready = true;
        }

        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead() || !LocalPlayer().Is(RoleEnum.Deputy)) return;    
            var depRole = GetRole<Deputy>(LocalPlayer());
            if (depRole.RemainingKills <= 0) return;
            
            for (int i = 0; i < __instance.playerStates.Length; i++)    
            {
                var playerState = __instance.playerStates[i];
                if (playerState.TargetPlayerId == LocalPlayer().PlayerId) continue;
                if (PlayerById(playerState.TargetPlayerId).Data.IsDead) continue;

                GenButton(depRole, i);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class DeputyVotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (LocalPlayer().Is(RoleEnum.Deputy))
            {
                var jailor = GetRole<Deputy>(LocalPlayer());
                jailor.ExecuteButton.Destroy();
            }
        }
    }
}