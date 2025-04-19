using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Vigilante : Role
    {
        private Dictionary<string, Color> ColorMapping = new Dictionary<string, Color>();
        public Dictionary<string, Color> SortedColorMapping;
        public int RemainingKills { get; set; }
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled { get; set; }

        public float VigilanteKillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.VigilanteKillCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            StartText = () => "Shoot or guess the <color=#FF0000FF>Killers</color>";
            TaskText = () => "Shoot or guess the <color=#FF0000FF>Impostor</color>";
            RoleInfo = "The Vigilante is able to kill players during rounds, if the player they kill is an impostor, the Vigilante will survive. If the player they kill is a crewmate, the Vigilante will die. During meetings, the Vigilante can guess the roles of players. If they get it right, the player will die. If they get it wrong, the Vigilante will die.";
            LoreText = "A lone avenger, you take matters into your own hands. As the Vigilante, you have the power to eliminate suspected Impostors with a single shot, or you can risk a guess and expose the killers hiding among the crew. Your quick thinking and decisiveness make you a powerful ally, but a mistake could have deadly consequences for the innocents.";
            Color = ColorManager.Vigilante;
            RoleType = RoleEnum.Vigilante;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewKilling;
            RemainingKills = CustomGameOptions.VigilanteKills;


            ColorMapping.Add("Impostor", ColorManager.ImpostorRed);
            if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", ColorManager.ImpostorRed);
            if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", ColorManager.ImpostorRed);
            if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", ColorManager.ImpostorRed);
            if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", ColorManager.ImpostorRed);
            if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", ColorManager.ImpostorRed);
            if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", ColorManager.ImpostorRed);
            if (CustomGameOptions.MinerOn > 0 && !IsFungleMap()) ColorMapping.Add("Miner", ColorManager.ImpostorRed);
            if (CustomGameOptions.MinerOn > 0 && IsFungleMap()) ColorMapping.Add("Herbalist", ColorManager.ImpostorRed);
            if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", ColorManager.ImpostorRed);
            if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", ColorManager.ImpostorRed);
            if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", ColorManager.ImpostorRed);
            if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", ColorManager.ImpostorRed);
            if (CustomGameOptions.BountyHunterOn > 0) ColorMapping.Add("Bounty Hunter", ColorManager.ImpostorRed);
            if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", ColorManager.ImpostorRed);
            if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", ColorManager.ImpostorRed);

            if (CustomGameOptions.AgentOn > 0) ColorMapping.Add("Hitman", ColorManager.Hitman);
            if (CustomGameOptions.AgentOn > 0) ColorMapping.Add("Agent", ColorManager.Agent); 
            if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", ColorManager.Arsonist);
            if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", ColorManager.Werewolf);
            if (CustomGameOptions.JuggernautOn > 0) ColorMapping.Add("Juggernaut", ColorManager.Juggernaut);
            if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Pestilence", ColorManager.Pestilence);
            if (CustomGameOptions.PlaguebearerOn > 0) ColorMapping.Add("Plaguebearer", ColorManager.Plaguebearer);
            if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("Glitch", ColorManager.Glitch);
            if (CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", ColorManager.Vampire);
            if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", ColorManager.SerialKiller);

            if (CustomGameOptions.VigilanteGuessNeutralBenign)
            {
                if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", ColorManager.GuardianAngel);
                if (CustomGameOptions.RomanticOn > 0) ColorMapping.Add("Romantic", ColorManager.Romantic);
                if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.RomanticOn > 0 && CustomGameOptions.RomanticOnBelovedDeath == RomanticBecomeOptions.Amnesiac)) ColorMapping.Add("Amnesiac", ColorManager.Amnesiac);
            }
            if (CustomGameOptions.VigilanteGuessNeutralEvil)
            {
                if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", ColorManager.Doomsayer);
                if (CustomGameOptions.VultureOn > 0) ColorMapping.Add("Vulture", ColorManager.Vulture);
                if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", ColorManager.Executioner);
                if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester)) ColorMapping.Add("Jester", ColorManager.Jester);
            }

            // Sorts the list
            SortedColorMapping = ColorMapping.ToDictionary(x => x.Key, x => x.Value);
        }
    }

    [HarmonyPatch]
    public class AddButtonVigilante
    {
        public static GameObject vigilanteUI;
        public static PassiveButton vigilanteUIExitButton;
        public static byte vigilanteCurrentTarget;
        public static bool IsExempt(PlayerVoteArea voteArea)
        {
            if (voteArea.AmDead) return true;
            var player = PlayerById(voteArea.TargetPlayerId);
            if (player.IsJailed()) return true;
            if (!PlayerControl.LocalPlayer.Is(Faction.Impostors))
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

        public static void vigilanteOnClick(int buttonTarget, MeetingHud __instance) 
        {
            var role = GetRole<Vigilante>(PlayerControl.LocalPlayer);
            if (vigilanteUI != null || !(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted)) return;
            __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(false));
            
            Transform PhoneUI = Object.FindObjectsOfType<Transform>().FirstOrDefault(x => x.name == "PhoneUI");
            Transform container = Object.Instantiate(PhoneUI, __instance.transform);
            container.transform.localPosition = new Vector3(0, 0, -5f);
            vigilanteUI = container.gameObject;

            int i = 0;
            var buttonTemplate = __instance.playerStates[0].transform.FindChild("votePlayerBase");
            var maskTemplate = __instance.playerStates[0].transform.FindChild("MaskArea");
            var smallButtonTemplate = __instance.playerStates[0].Buttons.transform.Find("CancelButton");
            var textTemplate = __instance.playerStates[0].NameText;

            vigilanteCurrentTarget = __instance.playerStates[buttonTarget].TargetPlayerId;

            Transform exitButtonParent = (new GameObject()).transform;
            exitButtonParent.SetParent(container);
            Transform exitButton = UnityEngine.Object.Instantiate(buttonTemplate.transform, exitButtonParent);
            Transform exitButtonMask = UnityEngine.Object.Instantiate(maskTemplate, exitButtonParent);
            exitButton.gameObject.GetComponent<SpriteRenderer>().sprite = smallButtonTemplate.GetComponent<SpriteRenderer>().sprite;
            exitButtonParent.transform.localPosition = new Vector3(2.725f, 2.1f, -5);
            exitButtonParent.transform.localScale = new Vector3(0.217f, 0.9f, 1);
            vigilanteUIExitButton = exitButton.GetComponent<PassiveButton>();
            vigilanteUIExitButton.OnClick.RemoveAllListeners();
            vigilanteUIExitButton.OnClick.AddListener((System.Action)(() => {
                __instance.playerStates.ToList().ForEach(x => {
                    x.gameObject.SetActive(true);
                    if (IsDead() && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject);
                });
                Object.Destroy(container.gameObject);
            }));

            List<Transform> buttons = new List<Transform>();
            Transform selectedButton = null;

            foreach (var pair in role.SortedColorMapping) 
            {
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
                    if (selectedButton != button) 
                    {
                        selectedButton = button;
                        buttons.ForEach(x => x.GetComponent<SpriteRenderer>().color = x == selectedButton ? Color.red : Color.white);
                    } 
                    else 
                    {
                        PlayerControl focusedTarget = PlayerById((byte)__instance.playerStates[buttonTarget].TargetPlayerId);
                        if (!(__instance.state == MeetingHud.VoteStates.Voted || __instance.state == MeetingHud.VoteStates.NotVoted) || focusedTarget == null || role.RemainingKills <= 0 ) return;

                        var mainRoleInfo = GetPlayerRole(focusedTarget);
                        var modRoleInfo = GetModifier(focusedTarget);

                        PlayerControl dyingTarget = (mainRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        if (modRoleInfo != null)
                            dyingTarget = (mainRoleInfo.Name == pair.Key || modRoleInfo.Name == pair.Key) ? focusedTarget : PlayerControl.LocalPlayer;
                        // Reset the GUI
                        __instance.playerStates.ToList().ForEach(x => x.gameObject.SetActive(true));
                        UnityEngine.Object.Destroy(container.gameObject);
                        if (CustomGameOptions.VigilanteMultiKill && role.RemainingKills > 1 && dyingTarget != PlayerControl.LocalPlayer)
                            __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == dyingTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        else
                            __instance.playerStates.ToList().ForEach(x => { if (x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });

                        // Shoot player
                        VigilanteKill.RpcMurderPlayer(role, dyingTarget, PlayerControl.LocalPlayer);
                        role.RemainingKills--;

                        if (dyingTarget.IsFortified()) 
                        {
                            dyingTarget = null;
                            Flash(ColorManager.Crusader, 1.5f);
                            __instance.playerStates.ToList().ForEach(x => { if (x.TargetPlayerId == focusedTarget.PlayerId && x.transform.FindChild("ShootButton") != null) UnityEngine.Object.Destroy(x.transform.FindChild("ShootButton").gameObject); });
                        }

                        
                    }
                }));
                i++;
            }
            container.transform.localScale *= 0.75f;
        }
    }

    public class VigilanteKill
    {
        public static void RpcMurderPlayer(Vigilante vigilanteP, PlayerControl player, PlayerControl vigilante)
        {
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            RpcMurderPlayer(vigilanteP, voteArea, player, vigilante);
        }
        public static void RpcMurderPlayer(Vigilante vigilanteP, PlayerVoteArea voteArea, PlayerControl player, PlayerControl vigilante)
        {
            MurderPlayer(vigilanteP, voteArea, player);
            VigiKillCount(player, vigilante);
            Utils.StartRPC(CustomRPC.VigilanteKill, player.PlayerId, vigilante.PlayerId);
        }

        public static void MurderPlayer(Vigilante vigilanteP, PlayerControl player)
        {
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );
            MurderPlayer(vigilanteP, voteArea, player);
        }
        public static void VigiKillCount(PlayerControl player, PlayerControl vigilante)
        {
            var vigi = GetRole<Vigilante>(vigilante);
            if (player != vigilante) vigi.CorrectVigilanteShot += 1;
        }
        public static void MurderPlayer(
            Vigilante vigilanteP,
            PlayerVoteArea voteArea,
            PlayerControl player)
        {
            var hudManager = HUDManager();
            var vigilantePlayer = vigilanteP.Player;

                try
                {
                    Sound().PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}
                if (PlayerControl.LocalPlayer== player) {
                    hudManager.KillOverlay.ShowKillAnimation(vigilantePlayer.Data, player.Data);
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
            }
            player.Die(DeathReason.Kill, false);
            
            GameHistory.CreateDeathReason(player, CustomDeathReason.Guess, vigilantePlayer);


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

            if (player.Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(PlayerControl.LocalPlayer);
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
            }

            if (player.Is(RoleEnum.Deputy))
            {
                var Deputy = GetRole<Deputy>(PlayerControl.LocalPlayer);
                Deputy.ExecuteButton.Destroy();
            }

            if (player.Is(RoleEnum.Imitator) && !player.Data.IsDead)
            {
                var imitatorRole = GetRole<Imitator>(player);
                if (MeetingHud.Instance.state != MeetingHud.VoteStates.Results && MeetingHud.Instance.state != MeetingHud.VoteStates.Proceeding)
                {
                    AddButtonImitator.GenButton(imitatorRole, voteArea, true);
                }
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

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class VigilanteCantReport
    {
        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.AmOwner) return;
            if (!__instance.CanMove) return;
            if (!__instance.Is(RoleEnum.Vigilante)) return;
            if (CustomGameOptions.VigilanteBodyReport) return;
            var truePosition = __instance.GetTruePosition();

            var data = __instance.Data;
            var stuff = Physics2D.OverlapCircleAll(truePosition, __instance.MaxReportDistance, Constants.Usables);
            var flag = (OptionsManager().currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) && __instance.CanMove;
            var flag2 = false;

            foreach (var collider2D in stuff)
                if (flag && !data.IsDead && !flag2 && collider2D.tag == "DeadBody")
                {
                    var component = collider2D.GetComponent<DeadBody>();

                    if (Vector2.Distance(truePosition, component.TruePosition) <= __instance.MaxReportDistance)
                    {
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != PlayerControl.LocalPlayer.PlayerId) { 
                            if (!PhysicsHelpers.AnythingBetween(__instance.Collider, truePosition, component.TruePosition, Constants.ShipOnlyMask, false)) flag2 = true; 
                        }
                    }
                }

            HUDManager().ReportButton.SetActive(flag2);
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportClosest))]
    public static class VigilanteDontReport
    {
        public static bool Prefix(PlayerControl __instance)
        {
            if (!__instance.Is(RoleEnum.Vigilante)) return true;
            if (CustomGameOptions.VigilanteBodyReport) return true;

            if (AmongUsClient.Instance.IsGameOver) return false;
            if (IsDead()) return false;
            foreach (var collider2D in Physics2D.OverlapCircleAll(__instance.GetTruePosition(),
                __instance.MaxReportDistance, Constants.PlayersOnlyMask))
                if (!(collider2D.tag != "DeadBody"))
                {
                    var component = collider2D.GetComponent<DeadBody>();
                    if (component && !component.Reported)
                    {
                        var matches = Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == component.ParentId);
                        if (matches != null && matches.KillerId != PlayerControl.LocalPlayer.PlayerId)
                            component.OnClick();
                        if (component.Reported) break;
                    }
                }

            return false;
        }
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class VigilanteHUDKill
    {
        private static KillButton KillButton;

        public static void Postfix(HudManager __instance)
        {
            KillButton = __instance.KillButton;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag7 = PlayerControl.AllPlayerControls.Count > 1;
            if (!flag7) return;
            var flag8 = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);
            if (flag8)
            {
                var role = GetRole<Vigilante>(PlayerControl.LocalPlayer);
                KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
                KillButton.SetCoolDown(role.VigilanteKillTimer(), CustomGameOptions.VigilanteKillCd);

                if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref role.ClosestPlayer, __instance.KillButton);
                else SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            }
            else
            {
                var isImpostor = PlayerControl.LocalPlayer.Data.IsImpostor();
                if (!isImpostor) return;

                __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

                if (IsHideNSeek()) return;
                ImpKillTarget(KillButton);
            }
        }

        public static void ImpKillTarget(KillButton killButton)
        {
            PlayerControl target = null;

            if (!PlayerControl.LocalPlayer.moveable) target = null;
            else if (CamouflageUnCamouflagePatch.IsCamouflaged && CustomGameOptions.CamoCommsKillAnyone) SetTarget(ref target, killButton);
            else SetTarget(ref target, killButton, float.NaN, AllPlayers().Where(x => !x.Is(Faction.Impostors)).ToList());
            killButton.SetTarget(target);
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public static class VigilanteKills
    {
        [HarmonyPriority(Priority.First)]
        private static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante);
            if (!flag) return true;
            var role = GetRole<Vigilante>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (IsDead()) return false;
            var flag2 = role.VigilanteKillTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled || role.ClosestPlayer == null) return false;
            var distBetweenPlayers = GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers < KillDistance();
            if (!flag3) return false;

            var flag4 = role.ClosestPlayer.Data.IsImpostor() || role.ClosestPlayer.Is(RoleAlignment.NeutralKilling)||
                        role.ClosestPlayer.Is(RoleAlignment.NeutralEvil) && CustomGameOptions.VigilanteKillsNeutralEvil ||
                        role.ClosestPlayer.Is(RoleAlignment.NeutralBenign) && CustomGameOptions.VigilanteKillsNeutralBenign;

            var abilityUsed = AbilityUsed(PlayerControl.LocalPlayer);
            if (!abilityUsed) return false;

            if (role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                return false;
            }
            if (role.ClosestPlayer.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }
            if (role.ClosestPlayer.IsOnAlert())
            {
                if (role.ClosestPlayer.IsShielded())
                {
                    var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                    StartRPC(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                    MedicStopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                    RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                }
                else if (role.Player.IsShielded())
                {
                    var medic = role.Player.GetMedic().Player.PlayerId;
                    StartRPC(CustomRPC.AttemptSound, medic, role.Player.PlayerId);
                    if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;
                    MedicStopKill.BreakShield(medic, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                    RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.VigilanteKillOther && !role.ClosestPlayer.IsProtected() && CustomGameOptions.KilledOnAlert)
                        RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                }
                else
                {
                    RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    if (CustomGameOptions.KilledOnAlert && CustomGameOptions.VigilanteKillOther)
                    {
                        RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    }
                }

                return false;
            }
            else if (role.ClosestPlayer == ShowRoundOneShield.FirstRoundShielded) return false;
            else if (role.ClosestPlayer.IsShielded())
            {
                var medic = role.ClosestPlayer.GetMedic().Player.PlayerId;
                StartRPC(CustomRPC.AttemptSound, medic, role.ClosestPlayer.PlayerId);

                if (CustomGameOptions.ShieldBreaks) role.LastKilled = DateTime.UtcNow;

                MedicStopKill.BreakShield(medic, role.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                return false;
            }
            else if (role.ClosestPlayer.IsProtected())
            {
                if (!flag4)
                {
                    RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                }
                role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                return false;
            }

            if (!flag4)
            {
                if (CustomGameOptions.VigilanteKillOther)
                    RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                RpcMurderPlayer(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            else
            {
                RpcMurderPlayer(PlayerControl.LocalPlayer, role.ClosestPlayer);
                role.LastKilled = DateTime.UtcNow;
            }
            return false;
        }
    }
}
