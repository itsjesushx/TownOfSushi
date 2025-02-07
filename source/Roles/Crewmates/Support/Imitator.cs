using UnityEngine.UI;

namespace TownOfSushi.Roles
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();
        public readonly List<(byte, bool)> ListOfActives = new List<(byte, bool)>();
        public PlayerControl ImitatePlayer = null;
        public PlayerControl LastExaminedPlayer = null;
        public List<RoleEnum> trappedPlayers = null;
        public Dictionary<byte, List<RoleEnum>> watchedPlayers = null;
        public PlayerControl confessingPlayer = null;
        public List<RoleEnum> ImitatableRoles = new List<RoleEnum>
        {
            RoleEnum.Investigator, RoleEnum.Mystic, RoleEnum.Seer, RoleEnum.Tracker, RoleEnum.Vigilante, 
            RoleEnum.Veteran, RoleEnum.Engineer, RoleEnum.Medium, RoleEnum.Trapper, 
            RoleEnum.Medic, RoleEnum.Oracle, RoleEnum.Hunter
        };
        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            StartText = () => "Use the true-hearted dead to benefit the crew";
            TaskText = () => "Use dead roles to benefit the crew";
            RoleInfo = "The Imitator is able to copy the abilities of dead crewmates to help the crew. The Imitator can not Imitate roles that work during meetings. This is the list of the roles that the imitator can Copy: Investigator, Mystic, Seer, Tracker, Vigilante, Veteran, Engineer, Medium, Trapper, Medic, Oracle, Hunter.";
            LoreText = "A master of adaptation, you possess the rare ability to channel the powers of the fallen. As the Imitator, you can use the abilities of the dead to aid the living, turning their sacrifices into a boon for the crew. Your unique talent allows you to adapt to any situation, bringing the strength of the departed back to the crew's side in the fight against the Impostors.";
            Color = ColorManager.Imitator;
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
        private static bool IsExempt(PlayerVoteArea voteArea)
        {
            var player = PlayerById(voteArea.TargetPlayerId);
            if (
                    player == null ||
                    !player.Data.IsDead ||
                    player.Data.Disconnected
                ) return true;
            return !player.Is(Faction.Crewmates);
        }
        public static void GenButton(Imitator role, PlayerVoteArea voteArea, bool replace = false)
        {
            if (LocalPlayer().IsJailed()) return;
            var targetId = voteArea.TargetPlayerId;
            if (IsExempt(voteArea))
            {
                if (replace) return;
                role.Buttons.Add(null);
                role.ListOfActives.Add((targetId, false));
                return;
            }

            var confirmButton = voteArea.Buttons.transform.GetChild(0).gameObject;

            var newButton = Object.Instantiate(confirmButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();
            var passive = newButton.GetComponent<PassiveButton>();

            renderer.sprite = DisabledSprite;
            newButton.transform.position = confirmButton.transform.position - new Vector3(0.75f, 0f, 0f);
            newButton.transform.localScale *= 0.8f;
            newButton.layer = 5;
            newButton.transform.parent = confirmButton.transform.parent.parent;

            passive.OnClick = new Button.ButtonClickedEvent();
            passive.OnClick.AddListener(SetActive(role, targetId));
            if (replace)
            {
                for (var i = 0; i < role.Buttons.Count; i++)
                {
                    if (role.ListOfActives[i].Item1 == targetId)
                    {
                        role.Buttons[i] = newButton;
                    }
                }
            }
            else
            {
                role.Buttons.Add(newButton);
                role.ListOfActives.Add((targetId, false));
            }
        }


        private static Action SetActive(Imitator role, int targetId)
        {
            void Listener()
            {
                int index = int.MaxValue;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (role.ListOfActives[i].Item1 == targetId)
                    {
                        index = i;
                        break;
                    }
                }
                if (index == int.MaxValue) return;
                if (role.ListOfActives.Count(x => x.Item2) == 1 &&
                    role.Buttons[index].GetComponent<SpriteRenderer>().sprite == DisabledSprite)
                {
                    int active = 0;
                    for (var i = 0; i < role.ListOfActives.Count; i++) if (role.ListOfActives[i].Item2) active = i;

                    role.Buttons[active].GetComponent<SpriteRenderer>().sprite =
                        role.ListOfActives[active].Item2 ? DisabledSprite : ActiveSprite;

                    role.ListOfActives[active] = (role.ListOfActives[active].Item1, !role.ListOfActives[active].Item2);
                }

                role.Buttons[index].GetComponent<SpriteRenderer>().sprite =
                    role.ListOfActives[index].Item2 ? DisabledSprite : ActiveSprite;

                role.ListOfActives[index] = (role.ListOfActives[index].Item1, !role.ListOfActives[index].Item2);

                _mostRecentId = index;

                SetImitate.Imitate = null;
                for (var i = 0; i < role.ListOfActives.Count; i++)
                {
                    if (!role.ListOfActives[i].Item2) continue;
                    SetImitate.Imitate = MeetingHud.Instance.playerStates[i];
                }
            }

            return Listener;
        }

        public static void Postfix(MeetingHud __instance)
        {
            foreach (var role in GetRoles(RoleEnum.Imitator))
            {
                var imitator = (Imitator)role;
                imitator.ListOfActives.Clear();
                imitator.Buttons.Clear();
            }

            if (IsDead()) return;
            if (!LocalPlayer().Is(RoleEnum.Imitator)) return;
            if (LocalPlayer().IsJailed()) return;
            var imitatorRole = Role.GetRole<Imitator>(LocalPlayer());
            foreach (var voteArea in __instance.playerStates)
            {
                GenButton(imitatorRole, voteArea);
            }
        }
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class ImitatorMeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (IsDead()) return;
            if (!LocalPlayer().Is(RoleEnum.Imitator)) return;
            var imitatorRole = GetRole<Imitator>(LocalPlayer());
            if (imitatorRole.LastExaminedPlayer != null)
            {
                if (CustomGameOptions.ExamineReportOn)
                {
                    var playerResults = MysticBodyReport.PlayerReportFeedback(imitatorRole.LastExaminedPlayer);
                    var roleResults = MysticBodyReport.RoleReportFeedback(imitatorRole.LastExaminedPlayer);

                    if (!string.IsNullOrWhiteSpace(playerResults)) Chat().AddChat(LocalPlayer(), playerResults);
                    if (!string.IsNullOrWhiteSpace(roleResults)) Chat().AddChat(LocalPlayer(), roleResults);
                }

                imitatorRole.LastExaminedPlayer = null;
            }
            else if (imitatorRole.trappedPlayers != null)
            {
                if (imitatorRole.trappedPlayers.Count == 0)
                {
                    Chat().AddChat(LocalPlayer(), "No players entered any of your traps");
                }
                else if (imitatorRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
                {
                    Chat().AddChat(LocalPlayer(), "Not enough players triggered your traps");
                }
                else
                {
                    string message = "Roles caught in your trap:\n";
                    foreach (RoleEnum role in imitatorRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" {role},";
                    }
                    message.Remove(message.Length - 1, 1);
                    if (HUDManager())
                        Chat().AddChat(LocalPlayer(), message);
                }
                imitatorRole.trappedPlayers.Clear();
            }
            else if (imitatorRole.confessingPlayer != null)
            {
                var playerResults = MeetingStartOracle.PlayerReportFeedback(imitatorRole.confessingPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) Chat().AddChat(LocalPlayer(), playerResults);
            }
            else if (imitatorRole.watchedPlayers != null)
            {
                foreach (var (key, value) in imitatorRole.watchedPlayers)
                {
                    var name = PlayerById(key).Data.PlayerName;
                    if (value.Count == 0)
                    {
                        if (HUDManager())
                            Chat().AddChat(LocalPlayer(), $"No players interacted with {name}");
                    }
                    else
                    {
                        string message = $"Roles seen interacting with {name}:\n";
                        foreach (RoleEnum role in value.OrderBy(x => Guid.NewGuid()))
                        {
                            message += $" {role},";
                        }
                        message = message.Remove(message.Length - 1, 1);
                        if (HUDManager())
                            Chat().AddChat(LocalPlayer(), message);
                    }
                }
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
            if (!LocalPlayer().Is(RoleEnum.Imitator)) return;
            if (IsDead() || LocalPlayer().Data.Disconnected) return;
            if (exiled == LocalPlayer()) return;

            var imitator = GetRole<Imitator>(LocalPlayer());
            if (imitator.ImitatePlayer == null) return;

            Imitate(imitator);

            StartRPC(CustomRPC.StartImitate, imitator.Player.PlayerId);
        }

        public static void Postfix(ExileController __instance) => ImitatorExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedLoaded || OptionsManager()?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ImitatorExileControllerPostfix(ExileControllerPatch.lastExiled);
        }

        public static void Imitate(Imitator imitator)
        {
            if (imitator.ImitatePlayer == null) return;
            ImitatingPlayer = imitator.Player;
            var imitatorRole = GetPlayerRole(imitator.ImitatePlayer).RoleType;
           // var keepRole = false;
            if (imitatorRole == RoleEnum.Mystic)
            {
                var Mystic = new Mystic(ImitatingPlayer);
                Mystic.LastExamined = Mystic.LastExamined.AddSeconds(CustomGameOptions.InitialExamineCd - CustomGameOptions.MysticExamineCd);
            }
            var role = GetPlayerRole(ImitatingPlayer);
            var killsList = (role.Kills, role.CorrectKills,  role.CorrectDeputyShot, role.CorrectShot, role.IncorrectShots, role.CorrectVigilanteShot, role.CorrectAssassinKills);
            RoleDictionary.Remove(ImitatingPlayer.PlayerId);
            if (imitatorRole == RoleEnum.Investigator) new Investigator(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Lookout) new Lookout(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Crewmate) new Crewmate(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Mystic) new Mystic(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Seer) new Seer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Tracker) new Tracker(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Veteran) new Veteran(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Vigilante) new Vigilante(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Detective) new Detective(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Deputy) new Deputy(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Jailor) new Jailor(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Swapper) new Swapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Engineer) new Engineer(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medium) new Medium(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Trapper) new Trapper(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Oracle) new Oracle(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Hunter) new Hunter(ImitatingPlayer);
            if (imitatorRole == RoleEnum.Medic)
            {
                var medic = new Medic(ImitatingPlayer);
                medic.UsedAbility = true;
                medic.StartingCooldown = medic.StartingCooldown.AddSeconds(-10f);
            }
            if (imitatorRole == RoleEnum.Crusader)
            {
                var crusader = new Crusader(ImitatingPlayer);
                crusader.StartingCooldown = crusader.StartingCooldown.AddSeconds(-10f);
            }

            var newRole = GetPlayerRole(ImitatingPlayer);
            newRole.RemoveFromRoleHistory(newRole.RoleType);
            newRole.Kills = killsList.Kills;
            newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
            newRole.CorrectKills = killsList.CorrectKills;
            newRole.IncorrectShots = killsList.IncorrectShots;
            newRole.CorrectShot = killsList.CorrectShot;
            newRole.CorrectDeputyShot = killsList.CorrectDeputyShot;
            newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
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
                Dictionary<byte, List<RoleEnum>> seenPlayers = null;
                PlayerControl confessingPlayer = null;

                if (LocalPlayer()== StartImitate.ImitatingPlayer)
                {
                    if (LocalPlayer().Is(RoleEnum.Engineer))
                    {
                        var engineerRole = GetRole<Engineer>(LocalPlayer());
                        Object.Destroy(engineerRole.UsesText);
                    }

                    if (LocalPlayer().Is(RoleEnum.Tracker))
                    {
                        var trackerRole = GetRole<Tracker>(LocalPlayer());
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        Object.Destroy(trackerRole.UsesText);
                    }

                    if (LocalPlayer().Is(RoleEnum.Mystic))
                    {
                        var mysticRole = GetRole<Mystic>(LocalPlayer());
                        mysticRole.BodyArrows.Values.DestroyAll();
                        mysticRole.BodyArrows.Clear();
                        lastExaminedPlayer = mysticRole.LastExaminedPlayer;
                    }

                    if (LocalPlayer().Is(RoleEnum.Veteran))
                    {
                        var veteranRole = GetRole<Veteran>(LocalPlayer());
                        Object.Destroy(veteranRole.UsesText);
                    }

                    if (LocalPlayer().Is(RoleEnum.Trapper))
                    {
                        var trapperRole = GetRole<Trapper>(LocalPlayer());
                        Object.Destroy(trapperRole.UsesText);
                        trapperRole.traps.ClearTraps();
                        trappedPlayers = trapperRole.trappedPlayers;
                    }

                    if (LocalPlayer().Is(RoleEnum.Lookout))
                    {
                        var loRole = GetRole<Lookout>(LocalPlayer());
                        Object.Destroy(loRole.UsesText);
                        seenPlayers = loRole.Watching;
                    }

                    if (LocalPlayer().Is(RoleEnum.Hunter))
                    {
                        var hunterRole = Role.GetRole<Hunter>(LocalPlayer());
                        Object.Destroy(hunterRole.UsesText);
                        hunterRole.ClosestPlayer = null;
                        hunterRole.ClosestStalkPlayer = null;
                        hunterRole.StalkButton.SetTarget(null);
                        hunterRole.StalkButton.gameObject.SetActive(false);
                    }

                    if (LocalPlayer().Is(RoleEnum.Oracle))
                    {
                        var oracleRole = GetRole<Oracle>(LocalPlayer());
                        oracleRole.ClosestPlayer = null;
                        confessingPlayer = oracleRole.Confessor;
                    }

                    if (LocalPlayer().Is(RoleEnum.Investigator))
                    {
                        Footprint.DestroyAll(GetRole<Investigator>(LocalPlayer()));
                        var detecRole = GetRole<Investigator>(LocalPlayer());
                        detecRole.ClosestPlayer = null;
                        detecRole.ExamineButton.gameObject.SetActive(false);
                    }

                    if (!LocalPlayer().Is(RoleEnum.Investigator) && !LocalPlayer().Is(RoleEnum.Mystic)
                        ) HUDManager().KillButton.gameObject.SetActive(false);
                }

                if (StartImitate.ImitatingPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = GetRole<Medium>(StartImitate.ImitatingPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                var role = GetPlayerRole(StartImitate.ImitatingPlayer);
                var killsList = (role.Kills, role.CorrectKills, role.IncorrectShots, role.CorrectShot, role.CorrectVigilanteShot, role.CorrectAssassinKills);
                RoleDictionary.Remove(StartImitate.ImitatingPlayer.PlayerId);
                var imitator = new Imitator(StartImitate.ImitatingPlayer);
                imitator.trappedPlayers = trappedPlayers;
                imitator.confessingPlayer = confessingPlayer;
                imitator.watchedPlayers = seenPlayers;
                imitator.LastExaminedPlayer = lastExaminedPlayer;
                var newRole = GetPlayerRole(StartImitate.ImitatingPlayer);
                newRole.RemoveFromRoleHistory(newRole.RoleType);
                newRole.Kills = killsList.Kills;
                newRole.CorrectKills = killsList.CorrectKills;
                newRole.IncorrectShots = killsList.IncorrectShots;
                newRole.CorrectShot = killsList.CorrectShot;
                newRole.CorrectVigilanteShot = killsList.CorrectVigilanteShot;
                newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                GetRole<Imitator>(StartImitate.ImitatingPlayer).ImitatePlayer = null;
                StartImitate.ImitatingPlayer = null;
            }
            return;
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

                if (LocalPlayer().Is(RoleEnum.Imitator))
                {
                    var imitator = GetRole<Imitator>(LocalPlayer());
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
                        StartRPC(CustomRPC.Imitate, imitator.Player.PlayerId, sbyte.MaxValue);
                        return;
                    }

                    StartRPC(CustomRPC.Imitate, imitator.Player.PlayerId, imitator.ImitatePlayer.PlayerId);
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
            if (LocalPlayer()== null) return;
            if (LocalPlayer().Data == null) return;
            if (IsDead()) return;
            if (StartImitate.ImitatingPlayer == null) return;
            if (LocalPlayer()!= StartImitate.ImitatingPlayer) return;
            if (!LocalPlayer().Is(RoleEnum.Vigilante) && !LocalPlayer().Is(RoleEnum.Hunter)) __instance.KillButton.OverrideText("");
            return;
        }
    }
}