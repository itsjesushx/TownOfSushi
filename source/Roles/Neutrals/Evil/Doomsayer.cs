using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Doomsayer : Role
    {
        private Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>();
        public Dictionary<string, Color> SortedColorMapping;
        public DateTime LastObserved;
        public PlayerControl ClosestPlayer;
        public PlayerControl LastObservedPlayer;
        public Doomsayer(PlayerControl player) : base(player)
        {
            Name = "Doomsayer";
            StartText = () => "Guess people's roles to win";
            TaskText = () => "Win by guessing player's roles";
            RoleInfo = "The Doomsayer is a Neutral role with its own win condition. Their goal is to assassinate a certain number of players. Once done so they win the game. They have an additional observe ability that hints towards certain player's roles.";
            LoreText = "A harbinger of fate, you see the truth in the chaos around you. As the Doomsayer, your task is to deduce the roles of others, using your sharp intuition and keen observation. Guessing correctly will lead to your victory, but each mistake brings you closer to defeat. Trust no one and use your knowledge wisely, for the game's outcome depends on your ability to uncover the truth behind every player’s role.";
            Color = ColorManager.Doomsayer;
            RoleType = RoleEnum.Doomsayer;
            LastObserved = DateTime.UtcNow;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;
            
            ColorMapping.Add("Crewmate", ColorManager.CrewmateBlue);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", ColorManager.Engineer);
            if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", ColorManager.Imitator);
            if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", ColorManager.Investigator);
            if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", ColorManager.Hunter);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", ColorManager.Medic);
            if (CustomGameOptions.CrusaderOn > 0) ColorMapping.Add("Crusader", ColorManager.Crusader);
            if (CustomGameOptions.AurialOn > 0) ColorMapping.Add("Aurial", ColorManager.Aurial);
            if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", ColorManager.Medium);
            if (CustomGameOptions.LookoutOn > 0) ColorMapping.Add("Lookout", ColorManager.Lookout);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", ColorManager.Swapper);
            if (CustomGameOptions.DeputyOn > 0) ColorMapping.Add("Deputy", ColorManager.Deputy);
            if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", ColorManager.Jailor);
            if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", ColorManager.Seer);
            if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", ColorManager.Mystic);
            if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", ColorManager.Oracle);
            if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", ColorManager.Detective);
            if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", ColorManager.Tracker);
            if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", ColorManager.Trapper);
            if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", ColorManager.Veteran);
            if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", ColorManager.Vigilante);

            if (CustomGameOptions.DoomsayerGuessNeutralBenign)
            {
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", ColorManager.GuardianAngel);
                if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", ColorManager.Romantic);
                if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", ColorManager.Amnesiac);
            }
            if (CustomGameOptions.DoomsayerGuessNeutralEvil)
            {
                if (!CustomGameOptions.UniqueRoles) ColorMapping.Add("Doomsayer", ColorManager.Doomsayer);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", ColorManager.Executioner);
                if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", ColorManager.Vulture);
                if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", ColorManager.Jester);
            }
            if (CustomGameOptions.DoomsayerGuessNeutralKilling)
            {
                if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) ColorMapping.Add("Hitman", ColorManager.Hitman);
                if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) ColorMapping.Add("Agent", ColorManager.Agent);
                if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", ColorManager.Arsonist);
                if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", ColorManager.Juggernaut);
                if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", ColorManager.Plaguebearer);
                if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", ColorManager.Glitch);
                if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", ColorManager.Werewolf);
                if (CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", ColorManager.Vampire);
                if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", ColorManager.SerialKiller);      
            }

            if (CustomGameOptions.DoomsayerGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
            {
                ColorMapping.Add("Impostor", ColorManager.ImpostorRed);
                if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", ColorManager.ImpostorRed);
                if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", ColorManager.ImpostorRed);
                if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", ColorManager.ImpostorRed);
                if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", ColorManager.ImpostorRed);
                if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", ColorManager.ImpostorRed);
                if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", ColorManager.ImpostorRed);
                if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", ColorManager.ImpostorRed);
                if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", ColorManager.ImpostorRed);
                if (CustomGameOptions.MinerOn > 0 && !IsFungleMap()) ColorMapping.Add("Miner", ColorManager.ImpostorRed);
                if (CustomGameOptions.MinerOn > 0 && IsFungleMap()) ColorMapping.Add("Herbalist", ColorManager.ImpostorRed);
                if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", ColorManager.ImpostorRed);
                if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", ColorManager.ImpostorRed);
                if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", ColorManager.ImpostorRed);
                if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", ColorManager.ImpostorRed);
                if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", ColorManager.ImpostorRed);
            }

            SortedColorMapping = ColorMapping.ToDictionary(x => x.Key, x => x.Value);
        }

        public float ObserveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastObserved;
            var num = CustomGameOptions.ObserveCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public int GuessedCorrectly = 0;
    }

    [HarmonyPatch]
    public class AddButtonDoomsayer
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

        public static void DoomsayerOnClick(int buttonTarget, MeetingHud __instance) 
        {
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
            doomsayerUIExitButton.OnClick.AddListener((System.Action)(() => 
            {
                __instance.playerStates.ToList().ForEach(x => 
                {
                    x.gameObject.SetActive(true);
                    if (IsDead() && x.transform.FindChild("ShootButton") != null) Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
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
                button.GetComponent<SpriteRenderer>().sprite = Ship().CosmeticsCache.GetNameplate("nameplate_NoPlate").Image;
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
                if (!IsDead() && !PlayerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId).Data.IsDead) button.GetComponent<PassiveButton>().OnClick.AddListener((System.Action)(() => {
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

    public class DoomsayerKill
    {
        public static void RpcMurderPlayer(Doomsayer doomsayerP, PlayerControl player, PlayerControl doomsayer)
        {
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(doomsayerP, voteArea, player, doomsayer);
        }
        public static void RpcMurderPlayer(Doomsayer doomsayerP, PlayerVoteArea voteArea, PlayerControl player, PlayerControl doomsayer)
        {
            MurderPlayer(doomsayerP, voteArea, player);
            DoomKillCount(player, doomsayer);
            Utils.StartRPC(CustomRPC.DoomsayerKill, player.PlayerId, doomsayer.PlayerId);
        }

        public static void MurderPlayer(Doomsayer doomsayerP, PlayerControl player)
        {
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(doomsayerP, voteArea, player);
        }
        public static void DoomKillCount(PlayerControl player, PlayerControl doomsayer)
        {
            var doom = GetRole<Doomsayer>(doomsayer);
            doom.CorrectAssassinKills += 1;
            doom.GuessedCorrectly += 1;
            if (doom.GuessedCorrectly == CustomGameOptions.DoomsayerGuessesToWin)
            {
                DoomsayerWin = true;
                StartRPC(CustomRPC.DoomsayerWin);
                EndGame();
            }
        }
        public static void MurderPlayer(
            Doomsayer doomsayerP, 
            PlayerVoteArea voteArea,
            PlayerControl player
        )
        {
            var hudManager = HUDManager();
            var doomsayerPlayer = doomsayerP.Player;

                try
                {
                    Sound().PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}
                if (PlayerControl.LocalPlayer== player) {
                    hudManager.KillOverlay.ShowKillAnimation(doomsayerPlayer.Data, player.Data);
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
                    for (int i = 0;i < player.myTasks.Count;i++)
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

                if (player.Is(RoleEnum.Swapper) && !player.Data.IsDead)
                {
                    var swapper = GetRole<Swapper>(player);
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
                if (player.Is(RoleEnum.Imitator) && !player.Data.IsDead)
                {
                    var imitatorRole = GetRole<Imitator>(player);
                    if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                    {
                        AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                    }
                }

                
            }
            player.Die(DeathReason.Kill, false);

            GameHistory.CreateDeathReason(player, CustomDeathReason.Guess, doomsayerPlayer);

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

            else if (player.Is(RoleEnum.Deputy))
            {
                var Deputy = GetRole<Deputy>(PlayerControl.LocalPlayer);
                Deputy.ExecuteButton.Destroy();
            }
            
            if (player.Is(RoleEnum.Deputy))
            {
                var Deputy = GetRole<Deputy>(PlayerControl.LocalPlayer);
                Deputy.ExecuteButton.Destroy();
            }

            foreach (var playerVoteArea in meetingHud.playerStates)
            {
                if (playerVoteArea.VotedFor != player.PlayerId) continue;
                playerVoteArea.UnsetVote();
                var voteAreaPlayer = PlayerById(playerVoteArea.TargetPlayerId);
                if (!voteAreaPlayer.AmOwner) continue;
                meetingHud.ClearVote();
            }

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AssassinExileControllerPatch.AssassinatedPlayers.Add(player);
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudObserve
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var role = GetRole<Doomsayer>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            
            __instance.KillButton.transform.localPosition = new Vector3(-2f, -0.06f, 0);
            __instance.KillButton.SetCoolDown(role.ObserveTimer(), CustomGameOptions.ObserveCooldown);
            SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class DoomsayerMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead()) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var doomsayerRole = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (doomsayerRole.LastObservedPlayer != null)
            {
                var playerResults = PlayerReportFeedback(doomsayerRole.LastObservedPlayer);
                var roleResults = RoleReportFeedback(doomsayerRole.LastObservedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) Chat().AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) Chat().AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                  || player.Is(RoleEnum.Glitch))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an altered perception of reality";
            else if (player.Is(RoleEnum.Blackmailer) ||player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Agent))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an insight for private information";
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) ||player.Is(RoleEnum.Hitman) || player.Is(RoleEnum.Vulture) || player.Is(RoleEnum.Vampire))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Lookout) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Detective))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic)|| player.Is(RoleEnum.Romantic))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} hides to guard themself or others";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 ||player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Veteran))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has a trick up their sleeve!";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.BountyHunter) ||player.Is(RoleEnum.Warlock))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is capable of performing relentless attacks!";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} appears to be roleless!";
            else
                return ColorString(ColorManager.ImpostorRed,"Error");
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Glitch))
                return "(" + ColorString(ColorManager.Imitator,"Imitator") + ", " + ColorString(ColorManager.ImpostorRed,"Morphling")  + ", " + ColorString(ColorManager.Aurial,"Aurial") + ", "+ ColorString(ColorManager.Mystic,"Mystic") + " or " + ColorString(ColorManager.Glitch,"Glitch") + ")";
            
            else if (player.Is(RoleEnum.Blackmailer)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Agent))
                return "(" + ColorString(ColorManager.ImpostorRed,"Blackmailer") + ", " + ColorString(ColorManager.Agent,"Agent") + ", " + ColorString(ColorManager.Seer,"Seer") + ", " + ColorString(ColorManager.Doomsayer,"Doomsayer") +", "+ ColorString(ColorManager.Oracle,"Oracle") + ", "+ ColorString(ColorManager.ImpostorRed,"Witch ") + " or " + ColorString(ColorManager.Trapper, "Trapper") + ")";
            
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Hitman) ||player.Is(RoleEnum.Vampire)|| player.Is(RoleEnum.Vulture) )
                return "("+ ColorString(ColorManager.Amnesiac,"Amnesiac") + ", " + ColorString(ColorManager.Vulture,"Vulture") + ", " + ColorString(ColorManager.ImpostorRed,"Janitor") +", "+ ColorString(ColorManager.Medium,"Medium") + ", "+ ColorString(ColorManager.ImpostorRed,"Undertaker ")+ ", "+ ColorString(ColorManager.Hitman,"Hitman ") + " or " + ColorString(ColorManager.Vampire,"Vampire") + ")";
            
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Lookout) || player.Is(RoleEnum.Tracker)
                 || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return "(" + ColorString(ColorManager.Investigator,"Investigator") + ", " + ColorString(ColorManager.ImpostorRed,"Swooper") + ", " + ColorString(ColorManager.Lookout,"Lookout") + ", " + ColorString(ColorManager.Tracker,"Tracker") + ", "+ ColorString(ColorManager.ImpostorRed,"Venerer ") + " or " + ColorString(ColorManager.SerialKiller,"Serial Killer") + ")";
            
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  ||player.Is(RoleEnum.Detective))
                return "(" + ColorString(ColorManager.Arsonist,"Arsonist") + ", " + ColorString(ColorManager.ImpostorRed,"Miner") + ", " + ColorString(ColorManager.Plaguebearer,"Plaguebearer") + " or "+ ColorString(ColorManager.Detective,"Detective ")+ ")";
            
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Romantic))
                return "(" + ColorString(ColorManager.Engineer,"Engineer") + ", " + ColorString(ColorManager.ImpostorRed,"Escapist") + ", " + ColorString(ColorManager.ImpostorRed,"Grenadier") +", " + ColorString(ColorManager.GuardianAngel,"Guardian Angel") + ", "+ ColorString(ColorManager.Medic,"Medic ") + " or " + ColorString(ColorManager.Romantic,"Romantic")+ ")";
            
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 ||player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Veteran))
                return "(" + ColorString(ColorManager.Doomsayer,"Doomsayer") + ", " + ColorString(ColorManager.Jester,"Jester") + ", " + ColorString(ColorManager.Swapper,"Swapper") + ", " + ColorString(ColorManager.Hunter,"Hunter") + " or " + ColorString(ColorManager.Veteran,"Veteran")+ ")";
            
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.BountyHunter) || player.Is(RoleEnum.Warlock))
                return "(" + ColorString(ColorManager.ImpostorRed,"Bomber") + ", " + ColorString(ColorManager.Juggernaut,"Juggernaut") + ", " + ColorString(ColorManager.ImpostorRed,"Bounty Hunter") + ", " + ColorString(ColorManager.Pestilence,"Pestilence") +", "+ ColorString(ColorManager.Vigilante,"Vigilante") + ", "+ ColorString(ColorManager.Vigilante,"Vigilante ") + " or " + ColorString(ColorManager.ImpostorRed,"Warlock")+ ")";
            
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(" + ColorString(ColorManager.ImpostorRed,"Impostor") + ", " + ColorString(ColorManager.CrewmateBlue,"Crewmate") + ")";
            
            else return ColorString(ColorManager.ImpostorRed,"Error");
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformObserve
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer);
            if (!flag) return true;
            if (IsDead()) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            var role = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (role.ObserveTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        NormalGameOptionsV09.KillDistances[OptionsManager().currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.LastObservedPlayer = role.ClosestPlayer;
            }
            if (interact[0] == true)
            {
                role.LastObserved = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastObserved = DateTime.UtcNow;
                role.LastObserved = role.LastObserved.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ObserveCooldown);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }
}
