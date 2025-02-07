using System.Collections;
using TMPro;
using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Jailor : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl Jailed;
        public bool CanJail;
        public GameObject ExecuteButton = new GameObject();
        public GameObject JailCell = new GameObject();
        public TMP_Text UsesText = new TMP_Text();
        public int Executes { get; set; }
        public Jailor(PlayerControl player) : base(player)
        {
            Name = "Jailor";
            StartText = () => "Jail and execute the <color=#FF0000FF>Killers</color>";
            TaskText = () => "Execute and speak to the <color=#FF0000FF>Killers</color>";
            RoleInfo = "The Jailor is able to jail a player during meetings, jailing a player automatically makes them unable to chat with anyone but the Jailor in meetings. The jailor can talk to their jailee by typing /jail in the chat. The Jailor may execute their jailee if they believe they are an Impostor. If the Jailor executes an innocent player, they lose the ability to jail and execute for the rest of the game.";
            LoreText = "A stern enforcer of justice, you specialize in imprisoning and executing the Impostors that threaten the crew. As the Jailor, you have the power to confine suspected killers and interrogate them before making the final, irreversible decision. Your sense of duty and unwavering resolve make you a vital figure in maintaining order and eliminating the threat of the Impostors.";            
            Color = ColorManager.Jailor;
            LastJailed = DateTime.UtcNow;
            RoleAlignment = RoleAlignment.CrewKilling;
            RoleType = RoleEnum.Jailor;
            AddToRoleHistory(RoleType);
            Executes = CustomGameOptions.MaxExecutes;
            CanJail = true;
        }
        public DateTime LastJailed { get; set; }
        public float JailTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastJailed;
            var num = CustomGameOptions.JailCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddJailButtons
    {
        public static Sprite CellSprite => TownOfSushi.InJailSprite;
        public static Sprite ExecuteSprite => TownOfSushi.ExecuteSprite;

        public static void GenCell(Jailor role, PlayerVoteArea voteArea)
        {
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var parent = confirmButton.transform.parent.parent;

            var jailCell = Object.Instantiate(confirmButton, voteArea.transform);
            var cellRenderer = jailCell.GetComponent<SpriteRenderer>();
            var passive = jailCell.GetComponent<PassiveButton>();
            cellRenderer.sprite = CellSprite;
            jailCell.transform.localPosition = new Vector3(-0.95f, 0f, -2f);
            jailCell.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            jailCell.layer = 5;
            jailCell.transform.parent = parent;
            jailCell.transform.GetChild(0).gameObject.Destroy();

            passive.OnClick = new Button.ButtonClickedEvent();
            role.JailCell = jailCell;
        }

        public static void GenButton(Jailor role, int index)
        {
            var voteArea = Meeting().playerStates[index];
            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = ExecuteSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(Execute(role));
            role.ExecuteButton = newButton;

            var usesText = Object.Instantiate(voteArea.NameText, voteArea.transform);
            usesText.transform.localPosition = new Vector3(-0.22f, 0.12f, 0f);
            usesText.text = role.Executes + "";
            usesText.transform.localScale = usesText.transform.localScale * 0.65f;
            role.UsesText = usesText;
        }


        private static Action Execute(Jailor role)
        {
            void Listener()
            {
                role.ExecuteButton.Destroy();
                role.UsesText.Destroy();
                role.JailCell.Destroy();
                role.Executes -= 1;
                    if (role.Jailed.Is(Faction.Crewmates))
                    {
                        role.IncorrectShots += 1;
                        role.CanJail = false;
                        role.Executes = 0;
                        Flash(Color.red);
                    }
                    else
                    {
                        role.CorrectKills += 1;
                        Flash(Color.green);
                    }
                    ExecuteKill(role, role.Jailed);
                    StartRPC(CustomRPC.Jail, role.Player.PlayerId, (byte)1);
                    role.Jailed = null;
            }

            return Listener;
        }

        public static void ExecuteKill(Jailor jailor, PlayerControl player)
        {
            PlayerVoteArea voteArea = Meeting().playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );

            var hudManager = HUDManager();

               try
                {
                    Sound().PlaySound(LocalPlayer().KillSfx, false, 1f);
                } catch {}
                if (LocalPlayer()== player) {
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
                        var imitator = Role.GetRole<Imitator>(LocalPlayer());
                        var buttons = Role.GetRole<Imitator>(player).Buttons;
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
                        var jailor2 = GetRole<Jailor>(LocalPlayer());
                        jailor2.ExecuteButton.Destroy();
                        jailor2.UsesText.Destroy();
                    }

                    if (player.Is(RoleEnum.Deputy))
                    {
                        var Deputy = GetRole<Deputy>(LocalPlayer());
                        Deputy.ExecuteButton.Destroy();
                    }
                
            }
            player.Die(DeathReason.Kill, false);

            GameHistory.CreateDeathReason(player, CustomDeathReason.Executed, jailor.Player);

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
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Jailor))
            {
                var jailor = (Jailor)role;
                jailor.JailCell.Destroy();
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
                if (jailor.Jailed == null) return;
                if (jailor.Player.Data.IsDead || jailor.Player.Data.Disconnected) return;
                if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                foreach (var voteArea in __instance.playerStates)
                    if (jailor.Jailed.PlayerId == voteArea.TargetPlayerId)
                    {
                        GenCell(jailor, voteArea);
                    }
            }

            if (IsDead()) return;
            if (!LocalPlayer().Is(RoleEnum.Jailor)) return;
            var jailorRole = GetRole<Jailor>(LocalPlayer());
            if (jailorRole.Executes <= 0 || jailorRole.Jailed.Data.IsDead || jailorRole.Jailed.Data.Disconnected) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
            if (jailorRole.Jailed.PlayerId == __instance.playerStates[i].TargetPlayerId)
            {
                GenButton(jailorRole, i);
            }
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class HudJail
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (!LocalPlayer().Is(RoleEnum.Jailor)) return;
            var jailButton = __instance.KillButton;

            var role = GetRole<Jailor>(LocalPlayer());

            jailButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !Meeting() && !IsDead()
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.CanJail)
            {
                jailButton.SetCoolDown(role.JailTimer(), CustomGameOptions.JailCd);
                SetTarget(ref role.ClosestPlayer, jailButton, float.NaN);
            }
            else
            {
                jailButton.SetCoolDown(0f, CustomGameOptions.JailCd);
                jailButton.SetTarget(null);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStartJailor
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead()) return;
            if (LocalPlayer().IsJailed())
            {
                if (LocalPlayer().Is(Faction.Crewmates))
                {
                    Chat().AddChat(LocalPlayer(), "You are jailed, provide relevant information to the Jailor to prove you are Crew");
                }
                else
                {
                    Chat().AddChat(LocalPlayer(), "You are jailed, convince the Jailor that you are Crew to avoid being executed");
                }
            }
            else if (LocalPlayer().Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(LocalPlayer());
                if (jailor.Jailed.Data.IsDead || jailor.Jailed.Data.Disconnected) return;
                Chat().AddChat(LocalPlayer(), "Use /jail to communicate with your jailee");
            }
        }
    }

    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformJail
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != HUDManager().KillButton) return true;
            var flag = LocalPlayer().Is(RoleEnum.Jailor);
            if (!flag) return true;
            var role = GetRole<Jailor>(LocalPlayer());
            if (!LocalPlayer().CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.JailTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = KillDistance();
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                LocalPlayer().GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Interact(LocalPlayer(), role.ClosestPlayer);
            if (interact[3] == true)
            {
                role.Jailed = role.ClosestPlayer;
                StartRPC(CustomRPC.Jail, LocalPlayer().PlayerId, (byte)0, role.Jailed.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastJailed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastJailed = DateTime.UtcNow;
                role.LastJailed = role.LastJailed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.JailCd);
                return false;
            }
            else if (interact[2] == true) return false;
            return false;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
    public static class JailorVotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (LocalPlayer().Is(RoleEnum.Jailor))
            {
                var jailor = GetRole<Jailor>(LocalPlayer());
                jailor.ExecuteButton.Destroy();
                jailor.UsesText.Destroy();
            }
        }
    }
}