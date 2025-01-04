namespace TownOfSushi.Roles
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<bool> ListOfActives = new List<bool>();
        public PlayerControl ImitatePlayer = null;
        public PlayerControl LastExaminedPlayer = null;
        public List<RoleEnum> trappedPlayers = null;
        public PlayerControl confessingPlayer = null;
        public List<RoleEnum> ImitatableRoles = new List<RoleEnum>
        {
            RoleEnum.Investigator, RoleEnum.Mystic, RoleEnum.Seer, RoleEnum.Tracker, RoleEnum.Vigilante, 
            RoleEnum.Veteran, RoleEnum.Engineer, RoleEnum.Medium, RoleEnum.Transporter, RoleEnum.Trapper, 
            RoleEnum.Medic, RoleEnum.Oracle, RoleEnum.Hunter
        };
        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            StartText = () => "Use the true-hearted dead to benefit the crew";
            TaskText = () => "Use dead roles to benefit the crew";
            RoleInfo = "The Imitator is able to copy the abilities of dead crewmates to help the crew. The Imitator can not Imitate roles that work during meetings. This is the list of the roles that the imitator can Copy: Investigator, Mystic, Seer, Tracker, Vigilante, Veteran, Engineer, Medium, Transporter, Trapper, Medic, Oracle, Hunter.";
            LoreText = "A master of adaptation, you possess the rare ability to channel the powers of the fallen. As the Imitator, you can use the abilities of the dead to aid the living, turning their sacrifices into a boon for the crew. Your unique talent allows you to adapt to any situation, bringing the strength of the departed back to the crew's side in the fight against the Impostors.";
            Color = Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            Faction = Faction.Crewmates;
            AddToRoleHistory(RoleType);
            RoleAlignment = RoleAlignment.CrewSupport;
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class AddButtonImitator
    {
        private static int _mostRecentId;
        private static Sprite ActiveSprite => TownOfSushi.ImitateSelectSprite;
        public static Sprite DisabledSprite => TownOfSushi.ImitateDeselectSprite;


        public static void GenButton(Imitator role, int index, bool isUseable, bool replace = false)
        {
            if (!isUseable)
            {
                role.Buttons.Add(null);
                role.ListOfActives.Add(false);
                return;
            }

            var confirmButton = MeetingHud.Instance.playerStates[index].Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, MeetingHud.Instance.playerStates[index].transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, index));
            if (replace) role.Buttons[index] = newButton;
            else
            {
                role.Buttons.Add(newButton);
                role.ListOfActives.Add(false);
            }
        }


        private static Action SetActive(Imitator role, int index)
        {
            void Listener()
            {
                if (role.ListOfActives.Count(x => x) == 1 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                {
                    int active = 0;
                    for (var i = 0; i < role.ListOfActives.Count; i++) if (role.ListOfActives[i]) active = i;

                    role.Buttons[active].GetComponent<SpriteRenderer>().sprite =
                        role.ListOfActives[active] ? DisabledSprite : ActiveSprite;

                    role.ListOfActives[active] = !role.ListOfActives[active];
                }

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index] ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = !role.ListOfActives[index];

                _mostRecentId = index;

                SetImitate.Imitate = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i]) continue;
                    SetImitate.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Imitator))
            {
                var imitator = (Imitator)role;
                imitator.ListOfActives.Clear();
                imitator.Buttons.Clear();
            }

            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.IsJailed()) return;
            var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.PlayerId == __instance.playerStates[i].TargetPlayerId)
                    {
                        var imitatable = false;
                        var imitatedRole = GetPlayerRole(player).RoleType;
                        if (player.Data.IsDead && !player.Data.Disconnected && imitatorRole.ImitatableRoles.Contains(imitatedRole)) imitatable = true;
                        GenButton(imitatorRole, i, imitatable);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class ImitatorMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            var imitatorRole = GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitatorRole.LastExaminedPlayer != null)
            {
                if (CustomGameOptions.ExamineReportOn)
                {
                    var playerResults = MysticBodyReport.PlayerReportFeedback(imitatorRole.LastExaminedPlayer);
                    var roleResults = MysticBodyReport.RoleReportFeedback(imitatorRole.LastExaminedPlayer);

                    if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                    if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
                }

                imitatorRole.LastExaminedPlayer = null;
            }
            else if (imitatorRole.trappedPlayers != null)
            {
                if (imitatorRole.trappedPlayers.Count == 0)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No players entered any of your traps");
                }
                else if (imitatorRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Not enough players triggered your traps");
                }
                else
                {
                    string message = "Roles caught in your trap:\n";
                    foreach (RoleEnum role in imitatorRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message.Remove(message.Length - 1, 1);
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
                imitatorRole.trappedPlayers.Clear();
            }
            else if (imitatorRole.confessingPlayer != null)
            {
                var playerResults = MeetingStartOracle.PlayerReportFeedback(imitatorRole.confessingPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }
    }

    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class ImitatorAirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => StartImitate.ImitatorExileControllerPostfix(__instance);
    }
    
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class StartImitate
    {
        public static PlayerControl ImitatingPlayer;
        public static void ImitatorExileControllerPostfix(ExileController __instance)
        {
            var exiled = __instance.initData.networkedPlayer?.Object;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead || PlayerControl.LocalPlayer.Data.Disconnected) return;
            if (exiled == PlayerControl.LocalPlayer) return;

            var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitator.ImitatePlayer == null) return;

            Imitate(imitator);

            Rpc(CustomRPC.StartImitate, imitator.Player.PlayerId);
        }

        public static void Postfix(ExileController __instance) => ImitatorExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ImitatorExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static void Imitate(Imitator imitator)
        {
            if (imitator.ImitatePlayer == null) return;
            ImitatingPlayer = imitator.Player;
            var imitatorRole = GetPlayerRole(imitator.ImitatePlayer).RoleType;
            if (imitatorRole == RoleEnum.Mystic)
            {
                var Mystic = new Mystic(ImitatingPlayer);
                Mystic.LastExamined = Mystic.LastExamined.AddSeconds(CustomGameOptions.InitialExamineCd - CustomGameOptions.MysticExamineCd);
            }
            if (imitatorRole == RoleEnum.Crewmate) return;
            var role = GetPlayerRole(ImitatingPlayer);
            var killsList = (role.Kills, role.CorrectKills,  role.CorrectShot, role.IncorrectShots, role.CorrectVigilanteShot, role.CorrectAssassinKills, role.IncorrectAssassinKills);
            RoleDictionary.Remove(ImitatingPlayer.PlayerId);
            if (imitatorRole == RoleEnum.Investigator) new Investigator(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Mystic) new Mystic(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Seer) new Seer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Tracker) new Tracker(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Veteran) new Veteran(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Engineer) new Engineer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medium) new Medium(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Transporter) new Transporter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Trapper) new Trapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Oracle) new Oracle(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Hunter) new Hunter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medic)
            {
                var medic = new Medic(ImitatingPlayer);
                medic.UsedAbility = true;
                medic.StartingCooldown = medic.StartingCooldown.AddSeconds(-10f);
            }

            var newRole = GetPlayerRole(ImitatingPlayer);
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            newRole.Kills = killsList.Kills;
            newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectShots = killsList.IncorrectShots;
            newRole.CorrectShot = killsList.CorrectShot;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class ImitatortartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            if (StartImitate.ImitatingPlayer != null)
            {
                PlayerControl lastExaminedPlayer = null;
                List<RoleEnum> trappedPlayers = null;
                PlayerControl confessingPlayer = null;

                if (PlayerControl.LocalPlayer == StartImitate.ImitatingPlayer)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(GetRole<Investigator>(PlayerControl.LocalPlayer));

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                    {
                        var engineerRole = GetRole<Engineer>(PlayerControl.LocalPlayer);
                        Object.Destroy(engineerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                    {
                        var trackerRole = GetRole<Tracker>(PlayerControl.LocalPlayer);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        Object.Destroy(trackerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var mysticRole = GetRole<Mystic>(PlayerControl.LocalPlayer);
                        mysticRole.BodyArrows.Values.DestroyAll();
                        mysticRole.BodyArrows.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    {
                        var transporterRole = GetRole<Transporter>(PlayerControl.LocalPlayer);
                        Object.Destroy(transporterRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var Mystic = GetRole<Mystic>(PlayerControl.LocalPlayer);
                        lastExaminedPlayer = Mystic.LastExaminedPlayer;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                    {
                        var veteranRole = GetRole<Veteran>(PlayerControl.LocalPlayer);
                        Object.Destroy(veteranRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                    {
                        var trapperRole = GetRole<Trapper>(PlayerControl.LocalPlayer);
                        Object.Destroy(trapperRole.UsesText);
                        trapperRole.traps.ClearTraps();
                        trappedPlayers = trapperRole.trappedPlayers;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
                    {
                        var hunterRole = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                        Object.Destroy(hunterRole.UsesText);
                        hunterRole.ClosestPlayer = null;
                        hunterRole.ClosestStalkPlayer = null;
                        hunterRole.StalkButton.SetTarget(null);
                        hunterRole.StalkButton.gameObject.SetActive(false);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
                    {
                        var oracleRole = GetRole<Oracle>(PlayerControl.LocalPlayer);
                        oracleRole.ClosestPlayer = null;
                        confessingPlayer = oracleRole.Confessor;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
                    {
                        var detecRole = GetRole<Investigator>(PlayerControl.LocalPlayer);
                        detecRole.ClosestPlayer = null;
                        detecRole.ExamineButton.gameObject.SetActive(false);
                    }

                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator) && !PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)
                        ) DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }

                if (StartImitate.ImitatingPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = GetRole<Medium>(StartImitate.ImitatingPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                var role = GetPlayerRole(StartImitate.ImitatingPlayer);
                var killsList = (role.Kills, role.CorrectKills, role.IncorrectShots, role.CorrectShot, role.CorrectVigilanteShot, role.CorrectAssassinKills, role.IncorrectAssassinKills);
                RoleDictionary.Remove(StartImitate.ImitatingPlayer.PlayerId);
                var imitator = new Imitator(StartImitate.ImitatingPlayer);
                imitator.trappedPlayers = trappedPlayers;
                imitator.confessingPlayer = confessingPlayer;
                var newRole = GetPlayerRole(StartImitate.ImitatingPlayer);
                newRole.RemoveFromRoleHistory(newRole.RoleType);
                newRole.Kills = killsList.Kills;
                newRole.CorrectKills = killsList.CorrectKills;
                newRole.IncorrectShots = killsList.IncorrectShots;
                newRole.CorrectShot = killsList.CorrectShot;
                newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
                newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                GetRole<Imitator>(StartImitate.ImitatingPlayer).ImitatePlayer = null;
                StartImitate.ImitatingPlayer = null;
            }
            return;
        }
    }

    public class ShowHideButtonsImitator
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return true;
                var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
                foreach (var button in imitator.Buttons.Where(button => button != null))
                {
                    if (button.GetComponent<SpriteRenderer>().sprite == AddButtonImitator.DisabledSprite)
                        button.SetActive(false);

                    button.GetComponent<PassiveButton>().OnClick = new UnityEngine.UI.Button.ButtonClickedEvent();
                }

                if (imitator.ListOfActives.Count(x => x) == 1)
                {
                    for (var i = 0; i < imitator.ListOfActives.Count; i++)
                    {
                        if (!imitator.ListOfActives[i]) continue;
                        SetImitate.Imitate = __instance.playerStates[i];
                    }
                }

                return true;
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud))]
    public class SetImitate
    {
        public static PlayerVoteArea Imitate;

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (Imitate == null) return;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Imitator))
                {
                    var imitator = GetRole<Imitator>(PlayerControl.LocalPlayer);
                    foreach (var button in imitator.Buttons.Where(button => button != null)) button.SetActive(false);

                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.PlayerId == Imitate.TargetPlayerId) 
                        { 
                            imitator.ImitatePlayer = player;
                        }
                    }

                    if (Imitate == null)
                    {
                        Rpc(CustomRPC.Imitate, imitator.Player.PlayerId, sbyte.MaxValue);
                        return;
                    }

                    Rpc(CustomRPC.Imitate, imitator.Player.PlayerId, imitator.ImitatePlayer.PlayerId);
                }
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public static class MeetingHud_Start
        {
            public static void Postfix(MeetingHud __instance)
            {
                Imitate = null;
            }
        }
    }

    [HarmonyPatch(typeof(HudManager))]
    public class OverrideKillText
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (StartImitate.ImitatingPlayer == null) return;
            if (PlayerControl.LocalPlayer != StartImitate.ImitatingPlayer) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante) && !PlayerControl.LocalPlayer.Is(RoleEnum.Hunter)) __instance.KillButton.OverrideText("");
            return;
        }
    }
}