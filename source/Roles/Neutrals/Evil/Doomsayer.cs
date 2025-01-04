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
            StartText = () => "Guess People's Roles To Win!";
            TaskText = () => "Win by guessing player's roles";
            RoleInfo = "The Doomsayer is a Neutral role with its own win condition. Their goal is to assassinate a certain number of players. Once done so they win the game. They have an additional observe ability that hints towards certain player's roles.";
            LoreText = "A harbinger of fate, you see the truth in the chaos around you. As the Doomsayer, your task is to deduce the roles of others, using your sharp intuition and keen observation. Guessing correctly will lead to your victory, but each mistake brings you closer to defeat. Trust no one and use your knowledge wisely, for the game's outcome depends on your ability to uncover the truth behind every player’s role.";
            Color = Colors.Doomsayer;
            RoleType = RoleEnum.Doomsayer;
            LastObserved = DateTime.UtcNow;
            Faction = Faction.Neutral;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.NeutralEvil;

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
            {
                ColorMapping.Add("Crewmate", Colors.Crewmate);
                if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Engineer);
                if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", Colors.Imitator);
                if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", Colors.Investigator);
                if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", Colors.Hunter);
                if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Medic);
                if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
                if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Swapper);
                // this will be gone for now 
                //if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", Colors.Jailor);
                if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Mystic);
                if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", Colors.Oracle);
                if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", Colors.Seer);
                if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
                if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", Colors.Transporter);
                if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", Colors.Trapper);
                if (CustomGameOptions.VeteranOn > 0) ColorMapping.Add("Veteran", Colors.Veteran);
                if (CustomGameOptions.VigilanteOn > 0) ColorMapping.Add("Vigilante", Colors.Vigilante);

                if (CustomGameOptions.DoomsayerGuessNeutralBenign)
                {
                    if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", Colors.Romantic);
                    if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralEvil)
                {
                    if (CustomGameOptions.GameMode == GameMode.AllAny) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                    if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", Colors.Vulture);
                    if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", Colors.Jester);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralKilling)
                {
                    if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) ColorMapping.Add("Hitman", Colors.Hitman);
                    if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) ColorMapping.Add("Agent", Colors.Agent);
                    if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", Colors.Glitch);
                    if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                    
                }

                if (CustomGameOptions.DoomsayerGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                {
                    ColorMapping.Add("Impostor", Colors.Impostor);
                    if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", Colors.Impostor);
                    if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", Colors.Impostor);
                    if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", Colors.Impostor);
                    if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Impostor);
                    if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Impostor);
                    if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", Colors.Impostor);
                    if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Impostor);
                    if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Impostor);
                    if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", Colors.Impostor);
                    if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", Colors.Impostor);
                    if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", Colors.Impostor);
                    if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", Colors.Impostor);
                }
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
        public bool WonByGuessing = false;
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

    public class DoomsayerKill
    {
        public static void RpcMurderPlayer(Doomsayer doomsayerP, PlayerControl player, PlayerControl doomsayer)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(doomsayerP, voteArea, player, doomsayer);
        }
        public static void RpcMurderPlayer(Doomsayer doomsayerP, PlayerVoteArea voteArea, PlayerControl player, PlayerControl doomsayer)
        {
            MurderPlayer(doomsayerP, voteArea, player);
            DoomKillCount(player, doomsayer);
            Utils.Rpc(CustomRPC.DoomsayerKill, player.PlayerId, doomsayer.PlayerId);
        }

        public static void MurderPlayer(Doomsayer doomsayerP, PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
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
                doom.WonByGuessing = true;
            }
        }
        public static void MurderPlayer(
            Doomsayer doomsayerP, 
            PlayerVoteArea voteArea,
            PlayerControl player
        )
        {
            var hudManager = DestroyableSingleton<HudManager>.Instance;
            var doomsayerPlayer = doomsayerP.Player;

                try
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}
                if (PlayerControl.LocalPlayer == player) {
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

                if (player.Is(RoleEnum.Swapper))
                {
                    var swapper = GetRole<Swapper>(PlayerControl.LocalPlayer);
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
                    Rpc(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
                }

                
            }
            player.Die(DeathReason.Kill, false);

            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Guessed;
            role2.KilledBy = " By " + Utils.ColorString(Colors.Doomsayer, doomsayerP.PlayerName);
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
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            __instance.KillButton.SetCoolDown(role.ObserveTimer(), CustomGameOptions.ObserveCooldown);
            SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class DoomsayerMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer)) return;
            var doomsayerRole = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (doomsayerRole.LastObservedPlayer != null)
            {
                var playerResults = PlayerReportFeedback(doomsayerRole.LastObservedPlayer);
                var roleResults = RoleReportFeedback(doomsayerRole.LastObservedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                  || player.Is(RoleEnum.Glitch))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an altered perception of reality";
            else if (player.Is(RoleEnum.Blackmailer) ||player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Agent))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an insight for private information";
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) ||player.Is(RoleEnum.Hitman) || player.Is(RoleEnum.Vulture) || player.Is(RoleEnum.Vampire))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic)|| player.Is(RoleEnum.Romantic))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} hides to guard themself or others";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 ||player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Veteran))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} has a trick up their sleeve!";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} is capable of performing relentless attacks!";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"You observe that {player.GetDefaultOutfit().PlayerName} appears to be roleless!";
            else
                return ColorString(Colors.Impostor,"Error");
        }

        public static string RoleReportFeedback(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Glitch))
                return "(" + ColorString(Colors.Imitator,"Imitator") + ", " + ColorString(Colors.Impostor,"Morphling") +", "+ ColorString(Colors.Mystic,"Mystic") + " or " + ColorString(Colors.Glitch,"Glitch") + ")";
            
            else if (player.Is(RoleEnum.Blackmailer)|| player.Is(RoleEnum.Doomsayer)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Witch)|| player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Agent))
                return "(" + ColorString(Colors.Impostor,"Blackmailer") + ", " + ColorString(Colors.Agent,"Agent") + ", " + ColorString(Colors.Doomsayer,"Doomsayer") +", "+ ColorString(Colors.Oracle,"Oracle") + ", "+ ColorString(Colors.Impostor,"Witch ") + " or " + ColorString(Colors.Trapper, "Trapper") + ")";
            
            else if (player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.Hitman) ||player.Is(RoleEnum.Vampire)|| player.Is(RoleEnum.Vulture) )
                return "("+ ColorString(Colors.Amnesiac,"Amnesiac") + ", " + ColorString(Colors.Vulture,"Vulture") + ", " + ColorString(Colors.Impostor,"Janitor") +", "+ ColorString(Colors.Medium,"Medium") + ", "+ ColorString(Colors.Impostor,"Undertaker ")+ ", "+ ColorString(Colors.Hitman,"Hitman ") + " or " + ColorString(Colors.Vampire,"Vampire") + ")";
            
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Swooper) || player.Is(RoleEnum.Tracker)
                 || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.SerialKiller))
                return "(" + ColorString(Colors.Investigator,"Investigator") + ", " + ColorString(Colors.Impostor,"Swooper") + ", " + ColorString(Colors.Tracker,"Tracker") + ", "+ ColorString(Colors.Impostor,"Venerer ") + " or " + ColorString(Colors.SerialKiller,"SerialKiller") + ")";
            
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                  ||player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter))
                return "(" + ColorString(Colors.Arsonist,"Arsonist") + ", " + ColorString(Colors.Impostor,"Miner") + ", " + ColorString(Colors.Plaguebearer,"Plaguebearer") + ", "+ ColorString(Colors.Seer,"Seer ") + " or " + ColorString(Colors.Transporter,"Transporter") + ")";
            
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Escapist) || player.Is(RoleEnum.Grenadier)
                || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Romantic))
                return "(" + ColorString(Colors.Engineer,"Engineer") + ", " + ColorString(Colors.Impostor,"Escapist") + ", " + ColorString(Colors.Impostor,"Grenadier") +", " + ColorString(Colors.GuardianAngel,"Guardian Angel") + ", "+ ColorString(Colors.Medic,"Medic ") + " or " + ColorString(Colors.Romantic,"Romantic")+ ")";
            
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester)
                 ||player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Veteran))
                return "(" + ColorString(Colors.Doomsayer,"Doomsayer") + ", " + ColorString(Colors.Jester,"Jester") + ", " + ColorString(Colors.Swapper,"Swapper") + ", " + ColorString(Colors.Hunter,"Hunter") + " or " + ColorString(Colors.Veteran,"Veteran")+ ")";
            
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Pestilence)
                 || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Warlock))
                return "(" + ColorString(Colors.Impostor,"Bomber") + ", " + ColorString(Colors.Juggernaut,"Juggernaut") + ", " + ColorString(Colors.Pestilence,"Pestilence") +", "+ ColorString(Colors.Vigilante,"Vigilante") + ", "+ ColorString(Colors.Vigilante,"Vigilante ") + " or " + ColorString(Colors.Impostor,"Warlock")+ ")";
            
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(" + ColorString(Colors.Impostor,"Impostor") + ", " + ColorString(Colors.Crewmate,"Crewmate") + ")";
            
            else return ColorString(Colors.Impostor,"Error");
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformObserve
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.isActiveAndEnabled || __instance.isCoolingDown) return false;
            var role = GetRole<Doomsayer>(PlayerControl.LocalPlayer);
            if (role.ObserveTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
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
