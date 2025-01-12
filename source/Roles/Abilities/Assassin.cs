using UnityEngine.UI;

namespace TownOfSushi.Roles.Modifiers
{
    public class Assassin : Ability
    {
        private readonly Dictionary<string, Color> ColorMapping = new();
        public Dictionary<string, Color> SortedColorMapping = new();
        public int RemainingKills { get; set; }
        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            TaskText = () => "Guess and shoot players in meetings";
            Color = Colors.Impostor;
            AbilityType = AbilityEnum.Assassin;
            RemainingKills = CustomGameOptions.AssassinKills;
            
            // Adds all the roles that have a > 0 chance of being in the game.
            if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", Colors.Crewmate);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Engineer);
            if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", Colors.Imitator);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Swapper);
            if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", Colors.Hunter);
            if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", Colors.Investigator);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Medic);
            if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
            if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Mystic);
            // this will be gone for now 
                //if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", Colors.Jailor);
            if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", Colors.Oracle);
            if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", Colors.Detective);
            if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
            if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", Colors.Trapper);
            if (CustomGameOptions.VeteranOn > 0 ) ColorMapping.Add("Veteran", Colors.Veteran);
            if (CustomGameOptions.VigilanteOn > 0 ) ColorMapping.Add("Vigilante", Colors.Vigilante);
            
            if (CustomGameOptions.ArsonistOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) ColorMapping.Add("Arsonist", Colors.Arsonist);
            if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Hitman)) ColorMapping.Add("Hitman", Colors.Hitman);
            if (CustomGameOptions.AgentOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Agent)) ColorMapping.Add("Agent", Colors.Agent);
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
            if (CustomGameOptions.PlaguebearerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer)) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
            if (CustomGameOptions.PlaguebearerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence)) ColorMapping.Add("Pestilence", Colors.Pestilence);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
            if (CustomGameOptions.GlitchOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) ColorMapping.Add("Glitch", Colors.Glitch);
            if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) ColorMapping.Add("Vampire", Colors.Vampire);
            if (CustomGameOptions.SerialKillerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);

            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Impostor", Colors.Impostor);
            if (CustomGameOptions.BomberOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Bomber", Colors.Impostor);
            if (CustomGameOptions.BlackmailerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Blackmailer", Colors.Impostor);
            if (CustomGameOptions.EscapistOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Escapist", Colors.Impostor);
            if (CustomGameOptions.WitchOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Witch", Colors.Impostor);
            if (CustomGameOptions.GrenadierOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Grenadier", Colors.Impostor);
            if (CustomGameOptions.JanitorOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Janitor", Colors.Impostor);
            if (CustomGameOptions.MorphlingOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Morphling", Colors.Impostor);
            if (CustomGameOptions.MinerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Miner", Colors.Impostor);
            if (CustomGameOptions.SwooperOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Swooper", Colors.Impostor);
            if (CustomGameOptions.VenererOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Venerer", Colors.Impostor);
            if (CustomGameOptions.UndertakerOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Undertaker", Colors.Impostor);
            if (CustomGameOptions.WarlockOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.Impostors)) ColorMapping.Add("Warlock", Colors.Impostor);

            // Add Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", Colors.Romantic);
                if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
            }
            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", Colors.Vulture);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", Colors.Jester);
            }
            //Add modifiers
            if (CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", Colors.Bait);
            if (CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", Colors.Torch);
            if (CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", Colors.Multitasker);
            if (CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", Colors.Diseased);
            if (CustomGameOptions.AftermathOn > 0) ColorMapping.Add("Aftermath", Colors.Aftermath);
            if (CustomGameOptions.FrostyOn > 0) ColorMapping.Add("Frosty", Colors.Frosty);

            // Sorts the list. 
            SortedColorMapping = ColorMapping.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    [HarmonyPatch]
    public class AssassinAddButton
    {
        public static GameObject assassinUI;
        public static PassiveButton assassinUIExitButton;
        public static byte assassinCurrentTarget;
        public static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = PlayerById(voteArea.TargetPlayerId);
            if (player.IsJailed()) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                if (
                    player == null ||
                    player.Is(RoleEnum.Vampire) ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleAlignment.NeutralKilling))
            {
                if (
                    player == null ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            else
            {
                if (
                    player == null ||
                    player.Data.IsImpostor() ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            var role = GetPlayerRole(player);
            return role != null && role.Criteria();
        }

        public static void AssassinOnClick(int buttonTarget, MeetingHud __instance) 
        {
            var ability = GetAbility<Assassin>(PlayerControl.LocalPlayer);
            if (assassinUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));

            Transform PhoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            Transform container = Object.Instantiate(PhoneUI, __instance.transform);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            assassinUI = container.gameObject;

            int i = 0;
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;

            assassinCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

            Transform exitButtonParent = (new GameObject()).transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
            Transform exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            exitButtonParent.transform.localScale = new Vector3(0.217f, 0.9f, 1);
            assassinUIExitButton = exitButton.GetComponent<PassiveButton>();
            assassinUIExitButton.OnClick.RemoveAllListeners();
            assassinUIExitButton.OnClick.AddListener((System.Action)(() => {
                __instance.playerStates.ToList().ForEach(x => {
                    x.gameObject.SetActive(true);
                    if (PlayerControl.LocalPlayer.Data.IsDead && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });
                UnityEngine.Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new List<Transform>();
            Transform selectedButton = null;

            foreach (var pair in ability.SortedColorMapping) {
                Transform buttonParent = (new GameObject()).transform;
                buttonParent.SetParent(container);
                Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
                TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
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
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || focusedTarget == null || ability.RemainingKills <= 0 ) return;

                        var mainRoleInfo = GetPlayerRole(focusedTarget);
                        var modRoleInfo = GetModifier(focusedTarget);

                        PlayerControl dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        if (modRoleInfo != null)
                            dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        
                        if (PlayerControl.LocalPlayer.Is(ModifierEnum.DoubleShot)) 
                        {
                            dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                            if (modRoleInfo != null)
                                dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                            
                            var modifier = GetModifier<DoubleShot>(PlayerControl.LocalPlayer);
                            if (dyingTarget == PlayerControl.LocalPlayer)
                            {
                                if (!modifier.LifeUsed) {
                                    dyingTarget = null;
                                    Flash(Colors.Impostor, 1.5f);
                                    modifier.LifeUsed = true;
                                    __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == focusedTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                                }
                                else {
                                    dyingTarget = PlayerControl.LocalPlayer;
                                }
                            }
                        }

                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        if (CustomGameOptions.AssassinMultiKill && ability.RemainingKills > 1 && dyingTarget != PlayerControl.LocalPlayer)
                            __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        else
                            __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                        // Shoot player
                       /* if (!dyingTarget.Is(RoleEnum.Pestilence) || PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
                        {*/
                            AssassinKill.RpcMurderPlayer(ability, dyingTarget, PlayerControl.LocalPlayer);
                            ability.RemainingKills--;
                       // }
                    }
                }));
                i++;
            }
            container.transform.localScale *= 0.75f;
        }
    }

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
            StartRPC(CustomRPC.AssassinKill, player.PlayerId, assassin.PlayerId);
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
                    StartRPC(CustomRPC.SetSwaps, sbyte.MaxValue, sbyte.MaxValue);
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
            }
            player.Die(DeathReason.Kill, false);

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

            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Guessed;
            if (role2 != null)
            {
                role2.KilledBy = " By " + ColorString(Colors.Impostor, assassinPlayer.name);
            }

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
            
            if (player.Is(RoleEnum.Imitator) && !player.Data.IsDead)
            {
                var imitatorRole = GetRole<Imitator>(PlayerControl.LocalPlayer);
                if (!meetingHud.playerStates[player.PlayerId].DidVote)
                {
                    RoleEnum imitatedRole = GetPlayerRole(player).RoleType;
                    var imitatable = imitatorRole.ImitatableRoles.Contains(imitatedRole);
                    AddButtonImitator.GenButton(imitatorRole, player.PlayerId, imitatable, true);
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
