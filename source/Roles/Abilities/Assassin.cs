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
            Color = ColorManager.ImpostorRed;
            AbilityType = AbilityEnum.Assassin;
            RemainingKills = CustomGameOptions.AssassinKills;
            
            // Adds all the roles that have a > 0 chance of being in the game.
            if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", ColorManager.CrewmateBlue);
            if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", ColorManager.Engineer);
            if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", ColorManager.Imitator);
            if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", ColorManager.Swapper);
            if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", ColorManager.Seer);
            if (CustomGameOptions.HunterOn > 0) ColorMapping.Add("Hunter", ColorManager.Hunter);
            if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", ColorManager.Investigator);
            if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", ColorManager.Medic);
            if (CustomGameOptions.CrusaderOn > 0) ColorMapping.Add("Crusader", ColorManager.Crusader);
            if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", ColorManager.Medium);
            if (CustomGameOptions.LookoutOn > 0) ColorMapping.Add("Lookout", ColorManager.Lookout);
            if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", ColorManager.Mystic);

            // this will be gone for now 
            //if (CustomGameOptions.JailorOn > 0) ColorMapping.Add("Jailor", ColorManager.Jailor);
            //if (CustomGameOptions.DeputyOn > 0) ColorMapping.Add("Deputy", ColorManager.Deputy);

            if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", ColorManager.Oracle);
            if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", ColorManager.Detective);
            if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", ColorManager.Tracker);
            if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", ColorManager.Trapper);
            if (CustomGameOptions.VeteranOn > 0 ) ColorMapping.Add("Veteran", ColorManager.Veteran);
            if (CustomGameOptions.VigilanteOn > 0 ) ColorMapping.Add("Vigilante", ColorManager.Vigilante);
            
            if (CustomGameOptions.ArsonistOn > 0 && !LocalPlayer().Is(RoleEnum.Arsonist)) ColorMapping.Add("Arsonist", ColorManager.Arsonist);
            if (CustomGameOptions.AgentOn > 0 && !LocalPlayer().Is(RoleEnum.Hitman)) ColorMapping.Add("Hitman", ColorManager.Hitman);
            if (CustomGameOptions.AgentOn > 0 && !LocalPlayer().Is(RoleEnum.Agent)) ColorMapping.Add("Agent", ColorManager.Agent);
            if (!LocalPlayer().Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", ColorManager.Juggernaut);
            if (CustomGameOptions.PlaguebearerOn > 0 && !LocalPlayer().Is(RoleEnum.Plaguebearer)) ColorMapping.Add("Plaguebearer", ColorManager.Plaguebearer);
            if (CustomGameOptions.PlaguebearerOn > 0 && !LocalPlayer().Is(RoleEnum.Pestilence)) ColorMapping.Add("Pestilence", ColorManager.Pestilence);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", ColorManager.Werewolf);
            if (CustomGameOptions.GlitchOn > 0 && !LocalPlayer().Is(RoleEnum.Glitch)) ColorMapping.Add("Glitch", ColorManager.Glitch);
            if (!LocalPlayer().Is(RoleEnum.Vampire)) ColorMapping.Add("Vampire", ColorManager.Vampire);
            if (CustomGameOptions.SerialKillerOn > 0 && !LocalPlayer().Is(RoleEnum.SerialKiller)) ColorMapping.Add("Serial Killer", ColorManager.SerialKiller);

            if (!LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Impostor", ColorManager.ImpostorRed);
            if (CustomGameOptions.BomberOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Bomber", ColorManager.ImpostorRed);
            if (CustomGameOptions.BlackmailerOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Blackmailer", ColorManager.ImpostorRed);
            if (CustomGameOptions.EscapistOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Escapist", ColorManager.ImpostorRed);
            if (CustomGameOptions.WitchOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Witch", ColorManager.ImpostorRed);
            if (CustomGameOptions.GrenadierOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Grenadier", ColorManager.ImpostorRed);
            if (CustomGameOptions.JanitorOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Janitor", ColorManager.ImpostorRed);
            if (CustomGameOptions.MorphlingOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Morphling", ColorManager.ImpostorRed);
            if (CustomGameOptions.MinerOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Miner", ColorManager.ImpostorRed);
            if (CustomGameOptions.SwooperOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Swooper", ColorManager.ImpostorRed);
            if (CustomGameOptions.PoisonerOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Poisoner", ColorManager.ImpostorRed);
            if (CustomGameOptions.BountyHunterOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Bounty Hunter", ColorManager.ImpostorRed);
            if (CustomGameOptions.VenererOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Venerer", ColorManager.ImpostorRed);
            if (CustomGameOptions.UndertakerOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Undertaker", ColorManager.ImpostorRed);
            if (CustomGameOptions.WarlockOn > 0 && !LocalPlayer().Is(Faction.Impostors)) ColorMapping.Add("Warlock", ColorManager.ImpostorRed);

            // Add Neutral roles if enabled
            if (CustomGameOptions.AssassinGuessNeutralBenign)
            {
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", ColorManager.GuardianAngel);
                if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", ColorManager.Romantic);
                if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", ColorManager.Amnesiac);
            }
            if (CustomGameOptions.AssassinGuessNeutralEvil)
            {
                if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", ColorManager.Doomsayer);
                if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", ColorManager.Vulture);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", ColorManager.Executioner);
                if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", ColorManager.Jester);
            }
            //Add modifiers
            if (CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", ColorManager.Bait);
            if (CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", ColorManager.Torch);
            if (CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", ColorManager.Multitasker);
            if (CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", ColorManager.Diseased);
            if (CustomGameOptions.AftermathOn > 0) ColorMapping.Add("Aftermath", ColorManager.Aftermath);
            if (CustomGameOptions.FrostyOn > 0) ColorMapping.Add("Frosty", ColorManager.Frosty);

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
            if (LocalPlayer().Is(RoleEnum.Vampire))
            {
                if (
                    player == null ||
                    player.Is(RoleEnum.Vampire) ||
                    player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            }
            else if (LocalPlayer().Is(RoleAlignment.NeutralKilling))
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
            var ability = GetAbility<Assassin>(LocalPlayer());
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
            assassinUIExitButton.OnClick.AddListener((System.Action)(() => 
            {
                __instance.playerStates.ToList().ForEach(x => {
                    x.gameObject.SetActive(true);
                    if (IsDead() && x.transform.FindChild("ShootButton") != null) Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });
                Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new List<Transform>();
            Transform selectedButton = null;

            foreach (var pair in ability.SortedColorMapping) {
                Transform buttonParent = (new GameObject()).transform;
                buttonParent.SetParent(container);
                Transform button = UnityEngine.Object.Instantiate(buttonTemplate, buttonParent);
                Transform buttonMask = UnityEngine.Object.Instantiate(maskTemplate, buttonParent);
                TMPro.TextMeshPro label = UnityEngine.Object.Instantiate(textTemplate, button);
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
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || focusedTarget == null || ability.RemainingKills <= 0 ) return;

                        var mainRoleInfo = GetPlayerRole(focusedTarget);
                        var modRoleInfo = GetModifier(focusedTarget);

                        PlayerControl dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : LocalPlayer();
                        if (modRoleInfo != null)
                            dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : LocalPlayer();
                        
                        if (LocalPlayer().Is(ModifierEnum.DoubleShot)) 
                        {
                            dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : LocalPlayer();
                            if (modRoleInfo != null)
                                dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : LocalPlayer();
                            
                            var modifier = GetModifier<DoubleShot>(LocalPlayer());
                            if (dyingTarget == LocalPlayer())
                            {
                                if (!modifier.LifeUsed)
                                {
                                    dyingTarget = null;
                                    Flash(ColorManager.ImpostorRed, 1.5f);
                                    modifier.LifeUsed = true;
                                    __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == focusedTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                                }
                                else 
                                {
                                    dyingTarget = LocalPlayer();
                                }
                            }
                        }

                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        if (CustomGameOptions.AssassinMultiKill && ability.RemainingKills > 1 && dyingTarget != LocalPlayer())
                            __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        else
                            __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                        // Shoot player
                        AssassinKill.RpcMurderPlayer(ability, dyingTarget, LocalPlayer());
                        ability.RemainingKills--;
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
            PlayerVoteArea voteArea = Meeting().playerStates.First(
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
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(assassinP, voteArea, player);
        }
        public static void AssassinKillCount(PlayerControl player, PlayerControl assassin)
        {
            var assassinPlayer = GetPlayerRole(assassin);
            if (player != assassin) assassinPlayer.CorrectAssassinKills += 1;
        }
        public static void MurderPlayer(
            Assassin assassinP, 
            PlayerVoteArea voteArea,
            PlayerControl player
        )
        {
            var hudManager = HUDManager();
            var assassinPlayer = assassinP.Player;
            try
            {
                Sound().PlaySound(LocalPlayer().KillSfx, false, 1f);
            } 
            catch {}
                if (LocalPlayer()== player) 
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
                if (!OptionsManager().currentNormalGameOptions.GhostsDoTasks)
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

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Swapper) && !PlayerControl.LocalPlayer.Data.IsDead)
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

                if (player.Is(RoleEnum.Imitator))
                {
                    var imitator = GetRole<Imitator>(LocalPlayer());
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
            
            GameHistory.CreateDeathReason(player, CustomDeathReason.Guess, assassinPlayer);

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

            if (player.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(LocalPlayer());
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
            }

            if (player.Is(RoleEnum.Deputy) && !player.Data.IsDead)
            {
                var dep = GetRole<Deputy>(player);
                if (dep.ExecuteButton != null)
                {
                    dep.ExecuteButton.SetActive(false);
                    dep.ExecuteButton.GetComponent<PassiveButton>().OnClick = new Button.ButtonClickedEvent();
                }
            }
            
            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AssassinExileControllerPatch.AssassinatedPlayers.Add(player);
        }
    }
}
