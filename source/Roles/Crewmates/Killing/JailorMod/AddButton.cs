using UnityEngine.UI;
using TownOfSushi.Roles.Crewmates.Support.ImitatorRole;
using TownOfSushi.Roles.Crewmates.Support.MedicRole;
using TownOfSushi.Roles.Impostors.Support.BlackmailerRole;

namespace TownOfSushi.Roles.Crewmates.Killing.JailorMod
{
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
            var voteArea = MeetingHud.Instance.playerStates[index];
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
               /* if (!role.Jailed.Is(RoleEnum.Pestilence))
                {*/
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
                    Rpc(CustomRPC.Jail, role.Player.PlayerId, (byte)1);
                    role.Jailed = null;
                //}
            }

            return Listener;
        }

        public static void ExecuteKill (Jailor jailor, PlayerControl player)
        {
            PlayerVoteArea voteArea = MeetingHud.Instance.playerStates.First(
                x => x.TargetPlayerId == player.PlayerId
            );

            var hudManager = DestroyableSingleton<HudManager>.Instance;

               try
                {
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);
                } catch {}
                if (PlayerControl.LocalPlayer == player) {
                    hudManager.KillOverlay.ShowKillAnimation(player.Data, player.Data);
                    if (VigilanteRole.AddButton.vigilanteUI != null) VigilanteRole.AddButton.vigilanteUIExitButton.OnClick.Invoke();
                    if (Abilities.AbilityMod.AssassinAbility.AddButton.assassinUI != null) Abilities.AbilityMod.AssassinAbility.AddButton.assassinUIExitButton.OnClick.Invoke();
                    if (Neutral.Evil.DoomsayerRole.AddButton.doomsayerUI != null) Neutral.Evil.DoomsayerRole.AddButton.doomsayerUIExitButton.OnClick.Invoke();
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
                        var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
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

                    if (player.Is(RoleEnum.Mayor))
                    {
                        var mayor = GetRole<Mayor>(PlayerControl.LocalPlayer);
                        mayor.RevealButton.Destroy();
                    }

                    if (player.Is(RoleEnum.Jailor))
                    {
                        var jailor2 = GetRole<Jailor>(PlayerControl.LocalPlayer);
                        jailor2.ExecuteButton.Destroy();
                        jailor2.UsesText.Destroy();
                    }
                
            }
            player.Die(DeathReason.Kill, false);

            var role2 = GetPlayerRole(player);
            role2.DeathReason = DeathReasonEnum.Executed;
            role2.KilledBy = " By " + ColorString(Colors.Jailor, jailor.PlayerName);

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

            if (AmongUsClient.Instance.AmHost) meetingHud.CheckForEndVoting();

            AddHauntPatch.AssassinatedPlayers.Add(player);
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

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jailor)) return;
            var jailorRole = GetRole<Jailor>(PlayerControl.LocalPlayer);
            if (jailorRole.Executes <= 0 || jailorRole.Jailed.Data.IsDead || jailorRole.Jailed.Data.Disconnected) return;
            for (var i = 0; i < __instance.playerStates.Length; i++)
                if (jailorRole.Jailed.PlayerId == __instance.playerStates[i].TargetPlayerId)
                {
                   /* if (!(jailorRole.Jailed.IsLover() && PlayerControl.LocalPlayer.IsLover()))*/GenButton(jailorRole, i);
                }
        }
    }
}